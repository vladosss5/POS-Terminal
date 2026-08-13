using System;
using System.Threading.Tasks;
using Com.Sunyard.Api.Rfreader;
using Terminal.Core.Entities.Models;
using Terminal.Core.Interfaces;

namespace Terminal.Android.Services.Sunyard.SunyardCardReader;

/// <summary>
/// Слушатель событий считывателя карт.
/// Реализует интерфейс IOnRfListener и получает уведомления об обнаружении карты или ошибках.
/// </summary>
public class RfListener : IOnRfListener.Stub
{
    private readonly TaskCompletionSource<CardReadResult> _tcs;
    private readonly SunyardCardReaderService _service;
    private readonly ILoggingService _logger;
    private bool _completed;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public RfListener(
        TaskCompletionSource<CardReadResult> tcs,
        SunyardCardReaderService service,
        ILoggingService logger)
    {
        _tcs = tcs;
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Вызывается при обнаружении карты.
    /// </summary>
    public override void OnCardPass(int cardType)
    {
        if (_completed) 
            return;
        
        _completed = true;

        try
        {
            _logger.LogDebug($"Card detected, type code: {cardType}");

            var cardInfo = ParseCardInfo(cardType);
            _tcs.TrySetResult(CardReadResult.Success(cardInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing card data:\n{ex.Message}\n{ex.InnerException}");
            _tcs.TrySetResult(CardReadResult.HardwareError("Failed to read card data"));
        }
    }

    /// <summary>
    /// Вызывается при ошибке или тайм-ауте.
    /// </summary>
    public override void OnFail(int error, string? message)
    {
        if (_completed) 
            return;
        
        _completed = true;

        _logger.LogDebug($"Card read failed: {error} - {message}");

        var result = error switch
        {
            2 => CardReadResult.Timeout(),
            7 => CardReadResult.Cancelled(),
            _ => CardReadResult.HardwareError(SunyardCardReaderService.GetErrorMessage(error, message))
        };

        _tcs.TrySetResult(result);
    }

    private CardInfo ParseCardInfo(int cardType)
    {
        var type = SunyardCardReaderService.MapCardType(cardType);
        var rfReader = _service.GetRfReader();

        string uid = ReadCardUid(rfReader);
        byte[] rawData = ReadRawData(rfReader);

        return new CardInfo(uid, type, rawData)
        {
            AdditionalInfo = GetAdditionalInfo(rfReader)
        };
    }

    private string ReadCardUid(IRFCardReader? rfReader)
    {
        try
        {
            if (rfReader == null)
                return "UID_UNAVAILABLE";

            // Для MIFARE карт
            byte[] response = new byte[16];
            int result = rfReader.ActivateCard("M1", response);

            if (result == 0 && response.Length >= 4)
            {
                byte[] uidBytes = new byte[4];
                Array.Copy(response, 1, uidBytes, 0, 4);
                return BitConverter.ToInt32(uidBytes).ToString();
            }

            // Для CPU карт через APDU
            byte[] getUidCommand = [0xFF, 0xCA, 0x00, 0x00, 0x00];
            var apduResponse = rfReader.ExchangeApdu(getUidCommand);

            if (apduResponse?.Length > 2)
            {
                byte[] uidBytes = new byte[apduResponse.Length - 2];
                Array.Copy(apduResponse, 0, uidBytes, 0, apduResponse.Length - 2);
                return BitConverter.ToString(uidBytes).Replace("-", "");
            }

            return "UID_NOT_READABLE";
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to read card UID\n{ex.Message}\n{ex.InnerException}");
            return "UID_ERROR";
        }
    }

    private byte[] ReadRawData(IRFCardReader? rfReader)
    {
        try
        {
            if (rfReader == null)
                return [];

            byte[] block0 = new byte[16];
            int result = rfReader.ReadBlock(0, block0);
            return result == 0 ? block0 : [];
        }
        catch
        {
            return [];
        }
    }

    private string GetAdditionalInfo(IRFCardReader? rfReader)
    {
        try
        {
            if (rfReader == null)
                return string.Empty;

            byte[] response = new byte[16];
            int result = rfReader.ActivateCard("CPU", response);

            if (result == 0 && response.Length > 0)
            {
                return $"ATR: {BitConverter.ToString(response).Replace("-", " ")}";
            }

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}