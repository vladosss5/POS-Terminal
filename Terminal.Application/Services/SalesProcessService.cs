using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;
using Terminal.Core.IRepositories;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class SalesProcessService : ISalesProcessService
{
    /// <inheritdoc cref="ILogger"/>
    private readonly ILogger<SalesProcessService> _logger;
    
    /// <inheritdoc cref="IConfigurationService" />
    private readonly IConfigurationService _configurationService;
    
    /// <inheritdoc cref="IReceiptPrintService"/>
    private readonly IReceiptPrintService _receiptPrintService;
    
    /// <inheritdoc cref="ISellingRepository" />
    private readonly ISellingRepository _sellingRepository;

    /// <inheritdoc cref="IResourceCodeRepository" />
    private readonly IResourceCodeRepository _resourceCodeRepository;
    
    /// <inheritdoc cref="ISettingRepository" />
    private readonly ISettingRepository _settingRepository;
    
    /// <inheritdoc cref="IShiftService" />
    private readonly IShiftService _shiftService;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    /// <inheritdoc cref="ISalesReceiptMappingService" />
    private readonly ISalesReceiptMappingService _receiptMappingService;
    
    /// <inheritdoc cref="ISettingPaymentTypeMapper" />
    private readonly ISettingPaymentTypeMapper _settingPaymentTypeMapper;

    private readonly IAuthService _authService;


    /// <summary>
    /// Коллекция потенциальных покупок, т.к. в рамках одной покупки может быть только один ресурс.
    /// </summary>
    private List<Selling> Cart { get; set; } = [];

    
    public SalesProcessService(
        ILogger<SalesProcessService> logger,
        IConfigurationService configurationService, 
        IReceiptPrintService receiptPrintService, 
        ISellingRepository sellingRepository, 
        IResourceCodeRepository resourceCodeRepository, 
        ISalesReceiptMappingService receiptMappingService, 
        ISettingPaymentTypeMapper settingPaymentTypeMapper, IAuthService authService, ISettingRepository settingRepository, IShiftService shiftService, IParameterService parameterService)
    {
        _logger = logger;
        _configurationService = configurationService;
        _receiptPrintService = receiptPrintService;
        _sellingRepository = sellingRepository;
        _resourceCodeRepository = resourceCodeRepository;
        _receiptMappingService = receiptMappingService;
        _settingPaymentTypeMapper = settingPaymentTypeMapper;
        _authService = authService;
        _settingRepository = settingRepository;
        _shiftService = shiftService;
        _parameterService = parameterService;
    }

    /// <inheritdoc/>
    public Dictionary<string, (BasePaymentType BaseType, DerivedPaymentType DerivedType)> GetAvailablePaymentTypes()
    {
        var paymentTypes = _configurationService.CurrentSetting.PaymentTypes;

        if (paymentTypes == null)
            throw new Exception("Not found payment types configuration");
        
        var dtos = paymentTypes
            .Where(x => x.IsEnabled)
            .Select(_settingPaymentTypeMapper.SettingPaymentTypeToDto);
        
        var resultDictionary = dtos.ToDictionary(
            paymentType => paymentType.DisplayedName, 
            paymentType => (paymentType.BaseType, paymentType.DerivedType));
        
        return resultDictionary;
    }

    /// <inheritdoc/>
    public async Task<List<ResourceCode>> GetAvailableResourceCodesAsync()
    {
        var resourceList = await _resourceCodeRepository.GetShowedResourceCodesAsync();
        return resourceList;
    }

    /// <inheritdoc/>
    public async Task AddToCartAsync(ResourceCode resource)
    {
        var sale = new Selling
        {
            ResourceKey = resource.ResourceKey,
            ResourceCode = resource.ResourceKey,
            ResourceName = resource.ResourceName,
            SellingPrice = resource.ResourcePrice
        };
        
        Cart.Add(sale);
    }

    /// <inheritdoc/>
    public async Task SetAmount(long resourceCodeId, decimal amount, bool isMoney)
    {
        var sale = Cart.FirstOrDefault(x => x.ResourceKey == resourceCodeId);

        if (sale == null)
            throw new Exception("ResourceCode not found");

        sale.Amount = amount;
        
        if (isMoney)
        {
            sale.RequestedCost = Math.Round(amount, 2);
            sale.RequestedAmount = sale.RequestedCost / sale.Amount;
        }
        else
        {
            sale.RequestedAmount = Math.Round(amount, 3);
            sale.RequestedCost = sale.RequestedAmount / sale.Amount;
        }
    }

    /// <inheritdoc/>
    public async Task RemoveFromCartAsync(ResourceCode resource)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task SetPaymentTypeAsync(BasePaymentType baseType, DerivedPaymentType derivedType)
    {
        foreach (var sale in Cart)
        {
            sale.BaseType = baseType;
            sale.DerivedType = derivedType;
        }
    }
    
    /// <inheritdoc/>
    public async Task CompleteProcessAsync()
    {
        var user = _authService.CurrentUser;
        var shift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        
        // await _builder.SetIssuerNumber(); TODO: добавить логику считывания эмитента из топливной карты
        
        foreach (var sale in Cart)
        {
            var chekNumberSetting = await _settingRepository.GetByKeyAsync(SettingsKey.Sale);
            sale.CheckNumber = ++chekNumberSetting!.Value;
            
            await _settingRepository.UpdateAsync(chekNumberSetting);

            sale.PersonName = user?.Name;
            sale.PersonKey = user?.UserId;
            sale.ShiftKey = shift?.ShiftKey;
            sale.TerminalKey = long.Parse(terminalNumber);
        }

        await _sellingRepository.AddRangeAsync(Cart);
    }

    /// <summary>
    /// Печать чека о продаже.
    /// </summary>
    /// <param name="selling">Продажа.</param>
    private async Task PrintReceiptAsync(Selling selling)
    {
        var receipt = _receiptMappingService.MapSellingToSalesReceipt(selling);
        
        var printResult = await _receiptPrintService.PrintSalesReceiptAsync(receipt);
        
        _logger.LogInformation($"Чек отбит.\n Результаты печати: {printResult.Status}, {printResult.ErrorMessage}");
    }
}