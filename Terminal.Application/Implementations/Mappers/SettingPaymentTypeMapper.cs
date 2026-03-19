using Terminal.Application.Interfaces.Mappers;
using Terminal.Core.Enums;
using Terminal.Core.Models;

namespace Terminal.Application.Implementations.Mappers;

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