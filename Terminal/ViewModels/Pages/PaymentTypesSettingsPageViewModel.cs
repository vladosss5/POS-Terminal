using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;
using Terminal.Core.Models.Settings;

namespace Terminal.ViewModels.Pages;

/// <summary>
/// Бизнес логика страницы настройки типов оплаты.
/// </summary>
public class PaymentTypesSettingsPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;

    /// <inheritdoc cref="ISettingPaymentTypeMapper" />
    private readonly ISettingPaymentTypeMapper _settingPaymentTypeMapper;

    /// <summary>
    /// Коллекция типов оплаты.
    /// </summary>
    public ObservableCollection<PaymentTypeDto> PaymentTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    /// <summary>
    /// Конструктор.
    /// </summary>
    public PaymentTypesSettingsPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IConfigurationService configurationService, 
        ISettingPaymentTypeMapper settingPaymentTypeMapper) 
        : base(logger)
    {
        Title = "Типы оплат";
        
        _configurationService = configurationService;
        _settingPaymentTypeMapper = settingPaymentTypeMapper;

        InitializeData();
    }
    
    /// <summary>
    /// Сменить статус активности у типа оплаты.
    /// </summary>
    /// <param name="paymentType">Dto типа оплаты.</param>
    public async Task SwitchPaymentTypeStatus(PaymentTypeDto paymentType)
    {
        var paymentTypeIndex = PaymentTypes.IndexOf(paymentType);
        PaymentTypes[paymentTypeIndex].IsEnabled = !PaymentTypes[paymentTypeIndex].IsEnabled;

        var listSettingsPaymentType = PaymentTypes.Select(_settingPaymentTypeMapper.DtoToSettingPaymentType);
        _configurationService.CurrentSetting.PaymentTypes = listSettingsPaymentType.ToList();
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack()
    {
        Navigation.GoBack();
    }
    
    /// <summary>
    /// Инициализировать данные.
    /// </summary>
    private void InitializeData()
    {
        var paymentTypesFromConfig = _configurationService.CurrentSetting.PaymentTypes;

        var dtos = paymentTypesFromConfig.Select(_settingPaymentTypeMapper.SettingPaymentTypeToDto);
        
        PaymentTypes.AddRange(dtos);
    }
}