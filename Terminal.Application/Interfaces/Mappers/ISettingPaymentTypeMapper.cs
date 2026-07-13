using Terminal.Core.Entities.Models;
using Terminal.Core.Entities.Models.Settings;

namespace Terminal.Application.Interfaces.Mappers;

/// <summary>
/// Сервис сопоставления SettingPaymentType с PaymentTypeDto.
/// </summary>
public interface ISettingPaymentTypeMapper
{
    /// <summary>
    /// Преобразование из SettingPaymentType в PaymentTypeDto
    /// </summary>
    public PaymentTypeDto SettingPaymentTypeToDto(SettingPaymentType paymentType);

    /// <summary>
    /// Преобразование из PaymentTypeDto в SettingPaymentType
    /// </summary>
    public SettingPaymentType DtoToSettingPaymentType(PaymentTypeDto paymentTypeDto);
}