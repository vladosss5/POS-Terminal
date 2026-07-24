using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Terminal.Application.Dtos;
using Terminal.Application.Dtos.CardInfoRoot;
using Terminal.Application.Helpers;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Core.Interfaces;

namespace Terminal.ViewModels.Pages;

public partial class CardInfoPageViewModel : PageViewModelBase
{
    /// <inheritdoc cref="IDiscountingMethods" />
    private readonly IDiscountingMethods _discountingMethods;
    
    /// <inheritdoc cref="ICardReaderService" />
    private readonly ICardReaderService _cardReaderService;
    
    /// <summary>
    /// Токен отмены считывания карты.
    /// </summary>
    private CancellationTokenSource? _cardReadCts;
    
    [ObservableProperty] 
    public partial bool ShowCardInfo { get; set; }

    /// <summary>
    /// Информация по карте.
    /// </summary>
    [ObservableProperty]
    public partial string CardInfoText { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    public CardInfoPageViewModel(
        ILogger<PageViewModelBase> logger, 
        IDiscountingMethods discountingMethods, 
        ICardReaderService cardReaderService) 
        : base(logger)
    {
        _discountingMethods = discountingMethods;
        _cardReaderService = cardReaderService;

        _ = StartCardReaderAsync();
    }
    
    /// <summary>
    /// Запустить считыватель карты.
    /// </summary>
    private async Task StartCardReaderAsync()
    {
        if (_cardReadCts != null)
            await _cardReadCts?.CancelAsync()!;
        
        _cardReadCts = new CancellationTokenSource();

        try
        {
            var result = await _cardReaderService.ReadCardAsync(
                timeoutSeconds: 30,
                cancellationToken: _cardReadCts.Token);

            if (!result.IsSuccess)
                return;

            var request = GetRequestDto(int.Parse(result.Card!.Uid));
            var response = await _discountingMethods.GetCardInfoAsync(request);

            CardInfoText = XmlHelper.SerializeXml(response);
            ShowCardInfo = true;
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message, e.InnerException);
        }
    }

    private CardInfoRequestDto GetRequestDto(int cardNumber)
    {
        var request = new CardInfoRequestDto()
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
                        ElectronicNumber = cardNumber,
                        BonusMode = 1,
                        ApplicationSchemeType = CardApplicationSchemeType.Max,
                        IssuerNet = 1,
                        OrganizationCode = 2600,
                        PersonCode = 1,
                        CardType = 2
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
                Flags = "2",
            }
        };

        return request;
    }
    
    /// <summary>
    /// Переместиться на страницу назад.
    /// </summary>
    public void StepBack()
    {
        _ = _cardReadCts?.CancelAsync()!;
        Navigation!.NavigateTo<MainMenuPageViewModel>();
    }
}