using Terminal.Application.Interfaces.Mappers;
using Terminal.Core.Entities.Models;
using Terminal.Core.Entities.Models.Settings;
using Terminal.Core.Enums;

namespace Terminal.Application.Mappers;

/// <inheritdoc/>
public class SettingPaymentTypeMapper : ISettingPaymentTypeMapper
{
    /// <inheritdoc/>
    public PaymentTypeDto SettingPaymentTypeToDto(SettingPaymentType paymentType)
    {
        return new PaymentTypeDto
        {
            DisplayedName = paymentType.DisplayedName,
            BaseType = (BasePaymentType)paymentType.BaseType,
            DerivedType = (DerivedPaymentType)paymentType.DerivedType,
            IsEnabled = paymentType.IsEnabled
        };
    }

    /// <inheritdoc/>
    public SettingPaymentType DtoToSettingPaymentType(PaymentTypeDto paymentTypeDto)
    {
        return new SettingPaymentType
        {
            DisplayedName = paymentTypeDto.DisplayedName,
            BaseType = (int)paymentTypeDto.BaseType,
            DerivedType = (int)paymentTypeDto.DerivedType,
            IsEnabled = paymentTypeDto.IsEnabled
        };
    }
}