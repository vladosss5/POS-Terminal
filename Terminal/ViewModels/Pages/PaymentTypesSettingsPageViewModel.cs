using System.Collections.ObjectModel;
using System.Linq;
using AvaloniaEdit.Utils;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Models;

namespace Terminal.ViewModels.Pages;

public class PaymentTypesSettingsPageViewModel : PageViewModelBase
{
    private readonly IConfigurationService _configurationService;

    public ObservableCollection<PaymentTypeSetting> PaymentTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public PaymentTypesSettingsPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IConfigurationService configurationService) 
        : base(logger)
    {
        _configurationService = configurationService;

        InitializeData();
    }

    private void InitializeData()
    {
        var paymentTypesFromConfig = _configurationService.GetPaymentTypeSettings();
        PaymentTypes.AddRange(paymentTypesFromConfig);
    }

    public void SwitchPaymentTypeStatus(PaymentTypeSetting paymentType)
    {
        var paymentTypeFromCollectionIndex = PaymentTypes.IndexOf(paymentType);
        PaymentTypes[paymentTypeFromCollectionIndex].IsEnabled = !PaymentTypes[paymentTypeFromCollectionIndex].IsEnabled;
    }
}