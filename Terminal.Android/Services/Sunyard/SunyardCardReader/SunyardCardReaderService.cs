using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Com.Sunyard.Api;
using Com.Sunyard.Api.Rfreader;
using Microsoft.Extensions.Logging;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;
using OperationCanceledException = System.OperationCanceledException;

namespace Terminal.Android.Services.Sunyard.SunyardCardReader;

public class SunyardCardReaderService : Java.Lang.Object, ICardReaderService
{
    /// <summary>
    /// Сервис логирования.
    /// </summary>
    private readonly ILoggingService _logger;

    /// <inheritdoc cref="ISoundService" />
    private readonly ISoundService _soundService;
    
    private readonly Context _context;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    
    private const string ServicePackage = "com.sunyard.deviceservice";
    private const string ServiceAction = "com.sunyard.api.device_service";

    private IDeviceService? _deviceService;
    private IRFCardReader? _rfReader;
    private SunyardServiceConnection? _serviceConnection;
    private bool _isConnected;
    private readonly Lock _lock = new();

    /// <summary>
    /// Событие, возникающее при изменении состояния подключения к считывателю.
    /// </summary>
    public event EventHandler<bool>? ConnectionChanged;

    /// <summary>
    /// Событие, возникающее при изменении статуса операции считывания.
    /// </summary>
    public event EventHandler<CardReaderStatus>? StatusChanged;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SunyardCardReaderService(
        Context context,
        ILoggingService logger, 
        ISoundService soundService)
    {
        _context = context;
        _logger = logger;
        _soundService = soundService;
    }

    /// <inheritdoc />
    public async Task<CardReadResult> ReadCardAsync(
        int timeoutSeconds = 30,
        CancellationToken cancellationToken = default)
    {
        try
        {
            OnStatusChanged(CardReaderStatus.Connecting);
            await ConnectAsync(cancellationToken).ConfigureAwait(false);

            CheckConnection();

            OnStatusChanged(CardReaderStatus.WaitingCard);

            var tcs = new TaskCompletionSource<CardReadResult>();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            await using var registration = cts.Token.Register(() =>
            {
                try
                {
                    _rfReader?.StopWait();
                    tcs.TrySetResult(CardReadResult.Timeout());
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error stopping card wait:\n{ex.Message}\n{ex.InnerException}");
                }
            });

            var listener = new RfListener(tcs, this, _logger);
            _rfReader!.WaitRFCard(listener);

            var result = await tcs.Task.ConfigureAwait(false);

            OnStatusChanged(result.IsSuccess ? CardReaderStatus.SuccessfullyRead : CardReaderStatus.ErrorRead);

            _soundService.PlaySound(result.IsSuccess ? SoundType.Success : SoundType.Error);

            return result;
        }
        catch (OperationCanceledException)
        {
            OnStatusChanged(CardReaderStatus.OperationCancelled);
            return CardReadResult.Cancelled();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error during card reading\n{ex.Message}\n{ex.InnerException}");
            OnStatusChanged(CardReaderStatus.InternalError);
            return CardReadResult.HardwareError(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        if (_isConnected) 
            return;

        var tcs = new TaskCompletionSource<bool>();

        _serviceConnection = new SunyardServiceConnection(
            onConnected: deviceService =>
            {
                lock (_lock)
                {
                    _deviceService = deviceService;
                    try
                    {
                        var binder = _deviceService.RFCardReader;
                        _rfReader = IRFCardReader.Stub.AsInterface(binder);
                        _isConnected = true;
                        ConnectionChanged?.Invoke(this, true);
                        tcs.TrySetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(new Exception("Failed to get RF card reader object", ex));
                    }
                }
            },
            onDisconnected: () =>
            {
                lock (_lock)
                {
                    _isConnected = false;
                    _rfReader?.Dispose();
                    _rfReader = null;
                    _deviceService?.Dispose();
                    _deviceService = null;
                    ConnectionChanged?.Invoke(this, false);
                }
            });

        var intent = new Intent(ServiceAction);
        intent.SetPackage(ServicePackage);
        
        if (!_context.BindService(intent, _serviceConnection, Bind.AutoCreate))
        {
            tcs.TrySetException(new InvalidOperationException(
                "Failed to bind to Sunyard service. Make sure the service is installed."));
            return;
        }

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(10000, cancellationToken));
        
        if (completedTask != tcs.Task)
        {
            if (_serviceConnection != null)
                _context.UnbindService(_serviceConnection);
            throw new TimeoutException("Connection to Sunyard service timed out.");
        }

        await tcs.Task;
    }

    private void Disconnect()
    {
        if (!_isConnected || _serviceConnection == null)
            return;

        try
        {
            _context.UnbindService(_serviceConnection);
            _logger.LogInformation("Disconnected from card reader service.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error while disconnecting from card reader service:\n{ex.Message}\n{ex.InnerException}");
        }

        lock (_lock)
        {
            _isConnected = false;
            _rfReader = null;
            _deviceService = null;
            _serviceConnection = null;
            ConnectionChanged?.Invoke(this, false);
        }
    }

    private void CheckConnection()
    {
        if (_isConnected && _rfReader != null)
            return;

        _logger.LogError("Card reader is not connected.");
        throw new InvalidOperationException("Card reader is not connected.");
    }

    private void OnStatusChanged(CardReaderStatus status)
    {
        _logger.LogDebug($"Reader status: {status}");
        
        StatusChanged?.Invoke(this, status);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Disconnect();
            _connectionLock.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Внутреннее свойство для доступа к rfReader из слушателя
    /// </summary>
    internal IRFCardReader? GetRfReader() => _rfReader;

    /// <summary>
    /// Внутренний метод для конвертации типа карты
    /// </summary>
    internal static CardType MapCardType(int cardType) => cardType switch
    {
        0 => CardType.MifareClassic1K,
        1 => CardType.MifareClassic4K,
        2 => CardType.MifarePro,
        3 => CardType.MifareS50Pro,
        4 => CardType.MifareS70Pro,
        5 => CardType.CpuCard,
        _ => CardType.Unknown
    };

    /// <summary>
    /// Внутренний метод для получения сообщения об ошибке
    /// </summary>
    internal static string GetErrorMessage(int errorCode, string? message)
    {
        if (!string.IsNullOrEmpty(message))
            return message;

        return errorCode switch
        {
            1 => "No card detected",
            2 => "Timeout waiting for card",
            3 => "Unknown card type",
            5 => "Multiple cards detected",
            7 => "Operation cancelled",
            -8888 => "Communication timeout",
            _ => $"Unknown error (code: {errorCode})"
        };
    }
}