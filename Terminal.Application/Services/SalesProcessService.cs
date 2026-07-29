using Microsoft.Extensions.Logging;
using Terminal.Application.Dtos;
using Terminal.Application.Dtos.CardInfoRoot;
using Terminal.Application.Dtos.DebitRoot;
using Terminal.Application.Dtos.DiscountRoot;
using Terminal.Application.Helpers;
using Terminal.Application.Interfaces.Mappers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Entities.Models;
using Terminal.Core.Enums;
using Terminal.Core.Exceptions;
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
    
    /// <inheritdoc cref="IDiscountingMethods" />
    private readonly IDiscountingMethods _discountingMethods;

    /// <inheritdoc cref="IAuthService" />
    private readonly IAuthService _authService;


    /// <summary>
    /// Коллекция потенциальных покупок, т.к. в рамках одной покупки может быть только один ресурс.
    /// </summary>
    private List<Selling> Cart { get; set; } = [];

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SalesProcessService(
        ILogger<SalesProcessService> logger,
        IConfigurationService configurationService, 
        IReceiptPrintService receiptPrintService, 
        ISellingRepository sellingRepository, 
        IResourceCodeRepository resourceCodeRepository, 
        ISalesReceiptMappingService receiptMappingService, 
        ISettingPaymentTypeMapper settingPaymentTypeMapper, 
        IAuthService authService, 
        ISettingRepository settingRepository, 
        IShiftService shiftService, IParameterService parameterService, IDiscountingMethods discountingMethods)
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
        _discountingMethods = discountingMethods;
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
    public void AddToCart(ResourceCode resource)
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
    public void SetAmount(long resourceCodeId, decimal amount, bool isMoney)
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
    public void RemoveFromCart(ResourceCode resource)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetPaymentType(BasePaymentType baseType, DerivedPaymentType derivedType)
    {
        foreach (var sale in Cart)
        {
            sale.BaseType = baseType;
            sale.DerivedType = derivedType;
        }
    }

    /// <inheritdoc/>
    public async Task CalculateDiscountAsync(CardInfo cardInfo)
    {
        var cardInfoRequestDto = GetRequestDto(cardInfo.Uid);
        var cardInfoResponseDto = _discountingMethods.GetCardInfo(cardInfoRequestDto);

        var discountRequestDto = await GetDiscountRequestDto(cardInfoResponseDto);
        var discountResponseDto = _discountingMethods.CalculateDiscount(discountRequestDto);
        
        // Дебетование
        var debitRequestDto = GetDebitRequestDto(discountResponseDto, cardInfoResponseDto);

        for (var i = 1; i <= 3; i++)
        {
            var debitResponseDto = _discountingMethods.Debit(debitRequestDto);
        
            var viewTypeParameter = debitResponseDto.Request.ResultMessageExt?
                .Split("\r\n")
                .FirstOrDefault(x => x.Contains("ViewType"))?
                .Split('=')
                .Last();

            if (debitResponseDto.Request.ResultCodeExt == 65549 && viewTypeParameter == "3")
            {
                debitRequestDto.Parameters.Pin = "2815";
                continue;
            }
            
            break;
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
            sale.TerminalKey = long.Parse(terminalNumber!);

            // CalculateDiscounting(sale);
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

    private CardInfoDtoRequestDto GetRequestDto(string cardNumber)
    {
        var request = new CardInfoDtoRequestDto()
        {
            Request = new RequestDto
            {
                Command = DiscounterCommand.GetCardInfo,
                IssuerId = 1,
                ShopId = 1
            },
            CardInfoList = new CardInfoList
            {
                CardInfos = 
                [
                    new CardInfoDto
                    {
                        ElectronicNumber = int.Parse(cardNumber),
                        BonusMode = 1,
                        ApplicationSchemeType = CardApplicationSchemeType.Resource,
                        IssuerNet = 1,
                        OrganizationCode = 9999999,
                        PersonCode = 1,
                        CardType = 2,
                        IssuerCode = 1,
                        GraphicalNumber = "0"
                    }
                ]
            },
            Parameters = new ParamsDto
            {
                BonusProgram = -1,
                AdjustAmount = -1,
                AdjustAmountOnline = -1,
                UserTimeout = -1,
                ReadCard = -1,
                Version = 2,
                Gift = -1,
                PrintData = -1,
                PrintCommentData = -1,
                CouponData = -1,
                CurrencyType = "руб."
            },
            CartInfo = new CartInfoDto
            {
                Flags = 2,
            }
        };

        return request;
    }
    
    private async Task<DiscountRequestDto> GetDiscountRequestDto(CardInfoDtoResponseDto cardInfoResponseDto)
    {
        var request = new DiscountRequestDto
        {
            Request = new RequestDto
            {
                Command = DiscounterCommand.CalculateDiscount,
                IssuerId = cardInfoResponseDto.Request.IssuerId,
                ShopId = cardInfoResponseDto.Request.ShopId
            },
            CartInfoDto = cardInfoResponseDto.CartInfo,
            Parameters = cardInfoResponseDto.Parameters,
            CardInfoList = cardInfoResponseDto.CardInfoList
        };

        var id = 1;

        foreach (var selling in Cart)
        {
            var resourceCode = await _resourceCodeRepository.GetByResourceKeyAsync(selling.ResourceKey!.Value);

            if (resourceCode == null)
                throw new NotFoundException($"Не найден resource code с ключом {selling.ResourceKey!.Value}");
            
            request.SaleInfoList.SaleInfos.Add(new SaleInfoDto
            {
                ResourcePrice = resourceCode!.ResourcePrice ?? 0,
                RequestSum = selling.RequestedCost ?? 0,
                RequestAmount = Math.Round(selling.RequestedAmount ?? 0, 3),
                Density = (float)(selling.Density ?? 0.545000),
                Flags = selling.RequestFlags ?? 0,
                Id = id++,
                RequestId = id,
                ResourceSet = resourceCode.CollectionKey ?? 3,
                ResourceCode = resourceCode.ResourceKey,
                AquirerResourceCode = selling.ResourceCode ?? 0,
                BasePaymentType = (int)selling.BaseType!,
                DerivedPaymentType = (int)selling.DerivedType!,
                VolumeDigits = 3,
                InitialCardInfoIndex = 255,
                InitialModifierCardInfoIndex = 255,
                CalculatedCardInfoIndex = 255,
                CalculatedModifierCardInfoIndex = 255,
                VendorCode = selling.VendorKey ?? 0,
                TaxCode = 9,
                SalePrice = selling.RequestedCost ?? 0,
                DateTime = XmlHelper.DateTimeToXml(selling.TransactionDatetime ?? DateTime.Now),
                ResourceName = selling.ResourceName,
                TransactionGuid = Guid.Empty.ToString()
            });
        }

        return request;
    }
    
    private DebitRequestDto GetDebitRequestDto(
        DiscountResponseDto discountResponseDto, 
        CardInfoDtoResponseDto cardInfoResponseDto)
    {
        var result = new DebitRequestDto
        {
            Request = new RequestDto
            {
                Command = DiscounterCommand.OnlineConfirm,
                IssuerId = discountResponseDto.Request.IssuerId,
                ShopId = discountResponseDto.Request.ShopId
            },
            CartInfoDto = discountResponseDto.CartInfoDto,
            Parameters = discountResponseDto.Parameters
        };

        var index = 0;

        foreach (var cardInfo in cardInfoResponseDto.CardInfoList.CardInfos)
        {
            cardInfo.Index = index++;
            result.CardInfoList.CardInfos.Add(cardInfo);
        }
        
        foreach (var saleInfo in discountResponseDto.SaleInfoList.SaleInfos)
        {
            saleInfo.InitialCardInfoIndex = saleInfo.CalculatedCardInfoIndex;
            result.SaleInfoList.SaleInfos.Add(saleInfo);
        }
        
        return result;
    }
}