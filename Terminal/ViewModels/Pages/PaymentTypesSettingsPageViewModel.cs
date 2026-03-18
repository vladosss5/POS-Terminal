using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.ViewModels.Pages;

public class PaymentTypesSettingsPageViewModel : PageViewModelBase
{
    private readonly IConfigurationService _configurationService;

    private readonly ISettingPaymentTypeMapper _settingPaymentTypeMapper;

    public ObservableCollection<PaymentTypeDto> PaymentTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public PaymentTypesSettingsPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IConfigurationService configurationService, 
        ISettingPaymentTypeMapper settingPaymentTypeMapper) 
        : base(logger)
    {
        Title = "Типы оплат";
        
        _configurationService = configurationService;
        _settingPaymentTypeMapper = settingPaymentTypeMapper;

        _ = InitializeData();
    }

    private async Task InitializeData()
    {
        var paymentTypesFromConfig = await _configurationService
            .GetValueAsync<List<SettingPaymentType>>("PaymentTypes");

        var dtos = paymentTypesFromConfig.Select(_settingPaymentTypeMapper.SettingPaymentTypeToDto);
        
        PaymentTypes.AddRange(dtos);
    }
    
    /// <summary>
    /// Перейти к прошлому шагу.
    /// </summary>
    public void StepBack()
    {
        Navigation.GoBack();
    }
    
    public async Task SwitchPaymentTypeStatus(PaymentTypeDto paymentType)
    {
        var paymentTypeIndex = PaymentTypes.IndexOf(paymentType);
        PaymentTypes[paymentTypeIndex].IsEnabled = !PaymentTypes[paymentTypeIndex].IsEnabled;

        var listSettingsPaymentType = PaymentTypes.Select(_settingPaymentTypeMapper.DtoToSettingPaymentType);
        await _configurationService.SetValueAsync("PaymentTypes", listSettingsPaymentType);
    }
}