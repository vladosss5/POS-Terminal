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
    
    /// <inheritdoc cref="ISellingMappingService" />
    private readonly ISellingMappingService _receiptMappingService;
    
    /// <inheritdoc cref="ISettingPaymentTypeMapper" />
    private readonly ISettingPaymentTypeMapper _settingPaymentTypeMapper;
    
    /// <inheritdoc cref="IDiscountingMethods" />
    private readonly IDiscountingMethods _discountingMethods;

    /// <inheritdoc cref="IAuthService" />
    private readonly IAuthService _authService;

    /// <inheritdoc cref="IAuthService" />
    private readonly ISellingMappingService _sellingMappingService;

    /// <inheritdoc cref="IStepNotifierService" />
    private readonly IStepNotifierService _stepNotifierService;
    
    /// <inheritdoc cref="ICardReaderService" />
    private readonly ICardReaderService _cardReaderService;
    
    /// <summary>
    /// Токен отмены считывания карты.
    /// </summary>
    private CancellationTokenSource? _cardReadCts;

    /// <summary>
    /// Данные считанной карты.
    /// </summary>
    private CardInfo? CardInfo { get; set; }

    /// <summary>
    /// Кортеж с пин-кодом и статусом ввода.
    /// </summary>
    private (string, bool) PinAndReadyTuple { get; set; } = ("", false);
    
    /// <summary>
    /// Коллекция потенциальных покупок, т.к. в рамках одной покупки может быть только один ресурс.
    /// </summary>
    private List<Selling> Cart { get; set; } = [];

    /// <summary>
    /// Номера чеков обязательных для печати.
    /// </summary>
    private List<int> _printCheckNumbers = [];
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public SalesProcessService(
        ILogger<SalesProcessService> logger,
        IConfigurationService configurationService, 
        IReceiptPrintService receiptPrintService, 
        ISellingRepository sellingRepository, 
        IResourceCodeRepository resourceCodeRepository, 
        ISellingMappingService receiptMappingService, 
        ISettingPaymentTypeMapper settingPaymentTypeMapper, 
        IAuthService authService, 
        ISettingRepository settingRepository, 
        IShiftService shiftService, 
        IParameterService parameterService, 
        IDiscountingMethods discountingMethods, 
        ISellingMappingService sellingMappingService, 
        IStepNotifierService stepNotifierService, 
        ICardReaderService cardReaderService)
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
        _sellingMappingService = sellingMappingService;
        _stepNotifierService = stepNotifierService;
        _cardReaderService = cardReaderService;
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
            BasePrice = resource.ResourcePrice
        };
        
        Cart.Add(sale);
        
        _stepNotifierService.CompleteCurrentStep();
    }

    /// <inheritdoc/>
    public void SetAmount(int resourceCodeId, decimal amount, CalculatedField calculatedField)
    {
        var sale = Cart.FirstOrDefault(x => x.ResourceKey == resourceCodeId);
        if (sale == null)
            throw new Exception("ResourceCode not found");

        sale.CalculatedField = calculatedField;
        
        if (sale.CalculatedField == CalculatedField.Amount)
        {
            sale.RequestFlags = 4;
            sale.RequestedCost = amount;
            sale.RequestedAmount = PriceHelper.CalculateAmount(sale.BasePrice!.Value, amount);
        }
        else
        {
            sale.RequestedAmount = amount;
            sale.RequestedCost = PriceHelper.CalculatePrice(sale.BasePrice!.Value, amount);
        }
        
        _stepNotifierService.CompleteCurrentStep();
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
    public async Task CompleteProcessAsync()
    {
        var cardInfo = GetCardInfoFromDiscounting(CardInfo?.Uid);
        var discountResult = await CalculateDiscountAsync(cardInfo);
        var debitResponse = await DebitAsync(cardInfo, discountResult);
        
        await SaveToDataBaseAsync(debitResponse);
        await PrintReceiptAsync();
    }

    /// <inheritdoc/>
    public async Task ReadCardAsync()
    {
        _stepNotifierService.GoToStep(SaleProcessStep.CardReading);
        
        var result = new CardReadResult();
        var counter = 0;

        while (!result.IsSuccess && counter < 3)
        {
            try
            {
                if (_cardReadCts != null)
                    await _cardReadCts?.CancelAsync()!;

                _cardReadCts = new CancellationTokenSource();

                result = await _cardReaderService.ReadCardAsync(30, _cardReadCts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.InnerException, result.ErrorMessage);
            }
            finally
            {
                counter++;
            }
        }
        
        CardInfo = result.Card!;
    }

    /// <inheritdoc/>
    public void EnterPin(string pin)
    {
        PinAndReadyTuple = (pin, true);
    }
    
    /// <summary>
    /// Сохранение покупки в БД.
    /// </summary>
    /// <param name="debitResponse"></param>
    private async Task SaveToDataBaseAsync(DebitResponseDto debitResponse)
    {
        var sellingIntoSave = new List<Selling>();
        
        var user = _authService.CurrentUser;
        var shift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
        
        foreach (var sale in debitResponse.SaleInfoList.SaleInfos)
        {
            var saleDomain = _sellingMappingService.MapSaleInfoDtoToDomainModel(sale);
            
            var chekNumberSetting = await _settingRepository.GetByKeyAsync(SettingsKey.Sale);
            saleDomain.CheckNumber = ++chekNumberSetting!.Value;
            _printCheckNumbers.Add(saleDomain.CheckNumber!.Value);
            await _settingRepository.UpdateAsync(chekNumberSetting);
        
            saleDomain.PersonName = user?.Name;
            saleDomain.PersonKey = user?.UserId;
            saleDomain.ShiftKey = shift?.ShiftKey;
            saleDomain.TerminalKey = long.Parse(terminalNumber!);
        
            sellingIntoSave.Add(saleDomain);
        }
        
        await _sellingRepository.AddRangeAsync(sellingIntoSave);
    }

    /// <summary>
    /// Дебетование карты.
    /// </summary>
    /// <returns>Ответ от ПЦ с результатом дебетования.</returns>
    private async Task<DebitResponseDto> DebitAsync(CardInfoDtoResponseDto? cardInfo, DiscountResponseDto discountResult)
    {
        var debitRequestDto = GetDebitRequestDto(discountResult, cardInfo);
        var debitResponse = new DebitResponseDto();
        
        for (var i = 1; i <= 3; i++)
        {
            debitResponse = _discountingMethods.Debit(debitRequestDto);

            if (CheckPinRequest(debitResponse.Request))
            {
                _stepNotifierService.GoToStep(SaleProcessStep.EnteringPin);

                while (!PinAndReadyTuple.Item2)
                    await Task.Delay(100);

                debitRequestDto.Parameters.Pin = PinAndReadyTuple.Item1;
                
                continue;
            }

            break;
        }

        return debitResponse;
    }

    /// <summary>
    /// Проверка требования PIN-кода процессинговый центром.
    /// </summary>
    /// <param name="requestDto">Модель запроса из ответа из ПЦ.</param>
    /// <returns>Требуется или не требуется.</returns>
    private bool CheckPinRequest(RequestDto requestDto)
    {
        if (string.IsNullOrEmpty(requestDto.ResultMessageExt))
            return false;
        
        var viewTypeParameter = requestDto.ResultMessageExt
            .Split("\r\n")
            .FirstOrDefault(x => x.Contains("ViewType"))?
            .Split('=')
            .Last();

        return requestDto.ResultCodeExt == 65549 && viewTypeParameter == "3";
    }

    /// <summary>
    /// Предварительный расчёт скидок.
    /// </summary>
    private async Task<DiscountResponseDto> CalculateDiscountAsync(CardInfoDtoResponseDto? cardInfoDto)
    {
        var discountRequestDto = await GetDiscountRequestDto(cardInfoDto);
        var result = _discountingMethods.CalculateDiscount(discountRequestDto);
        
        return result;
    }
    
    /// <summary>
    /// Получить информацию по карте из ПЦ.
    /// </summary>
    /// <param name="cardUid">Электронный номер карты.</param>
    /// <returns>Информация по карте.</returns>
    private CardInfoDtoResponseDto? GetCardInfoFromDiscounting(string? cardUid)
    {
        if (string.IsNullOrEmpty(cardUid))
            return null;
        
        var cardInfoRequestDto = GetRequestDto(cardUid);
        var cardInfoResponseDto = _discountingMethods.GetCardInfo(cardInfoRequestDto);

        var typeCode = cardInfoResponseDto.Request.ResultMessageExt?
            .Split("\r\n")
            .FirstOrDefault(x => x.Contains("Type"))?
            .Split('=')
            .Last();
        
        if (cardInfoResponseDto.Request.ResultCodeExt != 65552 || typeCode is not ("3" or "4")) // TODO Уточнить природу ошибки.
            return cardInfoResponseDto;
        
        cardInfoRequestDto.Parameters.ReadCard = 4;
        cardInfoRequestDto.CardInfoList = cardInfoResponseDto.CardInfoList;
        cardInfoResponseDto = _discountingMethods.GetCardInfo(cardInfoRequestDto);

        return cardInfoResponseDto;
    }
    
    /// <summary>
    /// Печать чека о продаже.
    /// </summary>
    private async Task PrintReceiptAsync()
    {
        foreach (var checkNumber in _printCheckNumbers)
        {
            var selling = await _sellingRepository.GetSellingByCheckNumberAsync(checkNumber);
            if (selling == null)
            {
                _logger.LogError("Продажа с номером чека {checkNumber} не найдена.", checkNumber);
                continue;
            }
            
            var receipt = _receiptMappingService.MapSellingToSalesReceipt(selling);
            var printResult = await _receiptPrintService.PrintSalesReceiptAsync(receipt);
            
            _logger.LogInformation($"Чек отбит.\n Результаты печати: {printResult.Status}, {printResult.ErrorMessage}");
        }
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
                CurrencyType = "руб.",
            },
            CartInfo = new CartInfoDto
            {
                Flags = 2,
            }
        };

        return request;
    }
    
    private async Task<DiscountRequestDto> GetDiscountRequestDto(CardInfoDtoResponseDto? cardInfoResponseDto)
    {
        var request = new DiscountRequestDto
        {
            Request = new RequestDto
            {
                Command = DiscounterCommand.CalculateDiscount,
                IssuerId = cardInfoResponseDto!.Request.IssuerId,
                ShopId = cardInfoResponseDto.Request.ShopId
            },
            CartInfoDto = cardInfoResponseDto.CartInfo,
            Parameters = cardInfoResponseDto.Parameters,
        };

        var cardInfoIndex = 0;
        foreach (var cardInfo in cardInfoResponseDto.CardInfoList.CardInfos)
        {
            cardInfo.Index = cardInfoIndex++;
            request.CardInfoList.CardInfos.Add(cardInfo);
        }

        var id = 1;
        foreach (var selling in Cart)
        {
            var resourceCode = await _resourceCodeRepository.GetByResourceKeyAsync(selling.ResourceKey!.Value);

            if (resourceCode == null)
                throw new NotFoundException($"Не найден resource code с ключом {selling.ResourceKey!.Value}");
            
            request.SaleInfoList.SaleInfos.Add(new SaleInfoDto
            {
                ResourcePrice = resourceCode.ResourcePrice ?? 0,
                RequestSum = selling.RequestedCost ?? 0,
                RequestAmount = selling.RequestedAmount ?? 0,
                Density = (float)(selling.Density ?? 0.545000),
                Flags = selling.RequestFlags ?? 0,
                Id = id++,
                RequestId = id,
                ResourceSet = resourceCode.CollectionKey ?? 3,
                ResourceCode = resourceCode.ResourceKey,
                AcquirerResourceCode = selling.ResourceCode ?? 0,
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
            Parameters = discountResponseDto.Parameters,
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