using System;
using System.Threading.Tasks;
using Android.Content;
using Com.Sunyard.Api;
using Com.Sunyard.Api.System;
using Microsoft.Extensions.Logging;
using Terminal.Android.Services.Sunyard;

namespace Terminal.Android.Services.DisplayMode;

/// <summary>
/// Реализация сервиса управления полноэкранным режимом для терминалов Sunyard.
/// </summary>
public class SunyardDisplayModeService : Java.Lang.Object, IDisplayModeSettingService
{
    private readonly ILogger<SunyardDisplayModeService> _logger;
    private readonly Context _context;
    
    private IDeviceService? _deviceService;
    private ISystemManager? _systemManager;
    private SunyardServiceConnection? _serviceConnection;
    
    private bool _isConnected;
    private readonly object _lock = new();

    /// <summary>
    /// Возвращает true, если включён полноэкранный режим.
    /// </summary>
    public bool IsFullScreenMode { get; private set; }

    /// <summary>
    /// Событие, возникающее при изменении состояния подключения.
    /// </summary>
    public event EventHandler<bool>? ConnectionChanged;
    
    /// <summary>
    /// Событие, возникающее при изменении полноэкранного режима.
    /// </summary>
    public event EventHandler<bool>? FullScreenModeChanged;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SunyardDisplayModeService(
        Context context,
        ILogger<SunyardDisplayModeService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    #region Подключение к сервису Sunyard

    /// <summary>
    /// Подключается к системному сервису Sunyard.
    /// </summary>
    private async Task ConnectAsync()
    {
        if (_isConnected) return;

        var tcs = new TaskCompletionSource<bool>();

        _serviceConnection = new SunyardServiceConnection(
            onConnected: deviceService =>
            {
                lock (_lock)
                {
                    _deviceService = deviceService;
                    try
                    {
                        // Получаем объект системного менеджера
                        var systemBinder = _deviceService.SystemManager;
                        if (systemBinder == null)
                        {
                            throw new InvalidOperationException("SystemManager не доступен");
                        }
                        
                        _systemManager = ISystemManager.Stub.AsInterface(systemBinder);
                        _isConnected = true;
                        
                        // Проверяем текущее состояние при подключении
                        Task.Run(async () => 
                        {
                            var currentStatus = await GetFullScreenModeStatusInternalAsync();
                            if (IsFullScreenMode != currentStatus)
                            {
                                IsFullScreenMode = currentStatus;
                                FullScreenModeChanged?.Invoke(this, IsFullScreenMode);
                            }
                        });
                        
                        tcs.TrySetResult(true);
                        _logger.LogInformation("Успешное подключение к сервису Sunyard");
                        ConnectionChanged?.Invoke(this, true);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при получении SystemManager");
                        tcs.TrySetException(new Exception("Failed to get SystemManager", ex));
                    }
                }
            },
            onDisconnected: () =>
            {
                lock (_lock)
                {
                    _isConnected = false;
                    IsFullScreenMode = false;
                    _systemManager = null;
                    _deviceService = null;
                    
                    _logger.LogWarning("Отключение от сервиса Sunyard");
                    ConnectionChanged?.Invoke(this, false);
                    FullScreenModeChanged?.Invoke(this, false);
                }
            });

        var intent = new Intent("com.sunyard.api.device_service");
        intent.SetPackage("com.sunyard.deviceservice");
        
        if (!_context.BindService(intent, _serviceConnection, Bind.AutoCreate))
        {
            _logger.LogError("Не удалось привязаться к сервису Sunyard");
            tcs.TrySetException(new InvalidOperationException(
                "Failed to bind to Sunyard service. Make sure the service is installed."));
            return;
        }

        var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(10000));
        if (completedTask != tcs.Task)
        {
            if (_serviceConnection != null)
                _context.UnbindService(_serviceConnection);
                
            _logger.LogError("Таймаут подключения к сервису Sunyard");
            throw new TimeoutException("Connection to Sunyard service timed out.");
        }

        await tcs.Task;
    }

    /// <summary>
    /// Отключается от системного сервиса Sunyard.
    /// </summary>
    private void Disconnect()
    {
        if (!_isConnected || _serviceConnection == null) 
            return;
        
        try
        {
            // Если был включён полноэкранный режим, выключаем его
            if (IsFullScreenMode)
            {
                try
                {
                    ExitFullScreenModeAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ошибка при выходе из полноэкранного режима");
                }
            }
            
            _context.UnbindService(_serviceConnection);
            _logger.LogInformation("Закрыто соединение с сервисом Sunyard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отключении от сервиса Sunyard");
        }

        _isConnected = false;
        IsFullScreenMode = false;
        _systemManager = null;
        _deviceService = null;
        _serviceConnection = null;
        
        ConnectionChanged?.Invoke(this, false);
        FullScreenModeChanged?.Invoke(this, false);
    }

    /// <summary>
    /// Проверяет состояние подключения.
    /// </summary>
    private async Task EnsureConnectedAsync()
    {
        if (!_isConnected)
            await ConnectAsync();
            
        if (!_isConnected || _deviceService == null || _systemManager == null)
            throw new InvalidOperationException("Сервис Sunyard не подключён");
    }

    #endregion

    #region Реализация IDisplayModeSettingService

    /// <inheritdoc/>
    public async Task EnterFullScreenModeAsync()
    {
        try
        {
            await EnsureConnectedAsync();
            
            if (IsFullScreenMode)
            {
                _logger.LogInformation("Уже в полноэкранном режиме");
                return;
            }

            _logger.LogInformation("Переключение в полноэкранный режим");
            
            // Скрываем статус-бар (верхняя шторка)
            int result1 = _systemManager!.SetStatusBarInvisible(true);
            _logger.LogDebug("SetStatusBarInvisible вернул: {Result}", result1);
            
            // Скрываем навигационную панель (кнопки BACK, HOME, RECENT)
            int result2 = _systemManager.SetNavigationBarInvisible(true);
            _logger.LogDebug("SetNavigationBarInvisible вернул: {Result}", result2);
            
            // Скрываем иконки уведомлений
            int result3 = _systemManager.SetNotificationIconDisable(true);
            _logger.LogDebug("SetNotificationIconDisable вернул: {Result}", result3);
            
            // Отключаем отдельные кнопки (опционально)
            _systemManager.SetBackKeyDisable(true);
            _systemManager.SetHomeKeyDisable(true);
            _systemManager.SetRecentKeyDisable(true);
            
            // Небольшая задержка для применения настроек
            await Task.Delay(100);
            
            // Проверяем, что всё применилось
            bool isStatusHidden = _systemManager.IsStatusBarInvisible;
            bool isNavHidden = _systemManager.IsNavigationBarInvisible;
            
            if (isStatusHidden && isNavHidden)
            {
                IsFullScreenMode = true;
                FullScreenModeChanged?.Invoke(this, true);
                _logger.LogInformation("Полноэкранный режим успешно включён");
            }
            else
            {
                _logger.LogWarning("Не все элементы интерфейса скрыты. Статус-бар скрыт: {StatusBar}, Навигация скрыта: {NavBar}", 
                    isStatusHidden, isNavHidden);
                
                // Пробуем ещё раз
                await Task.Delay(200);
                _systemManager.SetStatusBarInvisible(true);
                _systemManager.SetNavigationBarInvisible(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при включении полноэкранного режима");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task ExitFullScreenModeAsync()
    {
        try
        {
            if (!_isConnected || _systemManager == null)
            {
                _logger.LogWarning("Невозможно выйти из полноэкранного режима: сервис не подключён");
                IsFullScreenMode = false;
                FullScreenModeChanged?.Invoke(this, false);
                return;
            }

            if (!IsFullScreenMode)
            {
                _logger.LogInformation("Уже не в полноэкранном режиме");
                return;
            }

            _logger.LogInformation("Выход из полноэкранного режима");
            
            // Показываем статус-бар
            _systemManager.SetStatusBarInvisible(false);
            
            // Показываем навигационную панель
            _systemManager.SetNavigationBarInvisible(false);
            
            // Показываем иконки уведомлений
            _systemManager.SetNotificationIconDisable(false);
            
            // Включаем кнопки обратно
            _systemManager.SetBackKeyDisable(false);
            _systemManager.SetHomeKeyDisable(false);
            _systemManager.SetRecentKeyDisable(false);
            
            // Небольшая задержка для применения настроек
            await Task.Delay(100);
            
            IsFullScreenMode = false;
            FullScreenModeChanged?.Invoke(this, false);
            _logger.LogInformation("Полноэкранный режим отключён");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выходе из полноэкранного режима");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task ToggleFullScreenModeAsync()
    {
        if (IsFullScreenMode)
            await ExitFullScreenModeAsync();
        else
            await EnterFullScreenModeAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> GetFullScreenModeStatusAsync()
    {
        await EnsureConnectedAsync();
        return await GetFullScreenModeStatusInternalAsync();
    }

    /// <summary>
    /// Внутренний метод для получения статуса без проверки подключения.
    /// </summary>
    private Task<bool> GetFullScreenModeStatusInternalAsync()
    {
        if (_systemManager == null)
            return Task.FromResult(false);
            
        var isStatusHidden = _systemManager.IsStatusBarInvisible;
        var isNavHidden = _systemManager.IsNavigationBarInvisible;
        var isFullScreen = isStatusHidden && isNavHidden;
        
        _logger.LogDebug("Текущее состояние - Статус-бар скрыт: {StatusBar}, Навигация скрыта: {NavBar}", 
            isStatusHidden, isNavHidden);
            
        return Task.FromResult(isFullScreen);
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Освобождает ресурсы сервиса.
    /// </summary>
    public void Dispose()
    {
        Disconnect();
        GC.SuppressFinalize(this);
    }

    #endregion
}