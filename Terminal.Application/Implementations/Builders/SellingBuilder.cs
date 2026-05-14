using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Enums;
using Terminal.Data.MainDB;

namespace Terminal.Application.Implementations.Builders;

/// <inheritdoc/>
public class SellingBuilder : ISellingBuilder
{
    private ILogger<SellingBuilder> _logger;
    
    /// <inheritdoc cref="Selling" />
    private readonly Selling _selling = new();

    /// <inheritdoc cref="IShiftService" />
    private readonly IShiftService _shiftService;
    
    /// <inheritdoc cref="IParameterService" />
    private readonly IParameterService _parameterService;
    
    /// Фабрика создающая <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SellingBuilder(
        ILogger<SellingBuilder> logger,
        IShiftService shiftService,
        IDbContextFactory<DataContext> dbFactory, 
        IParameterService parameterService)
    {
        _shiftService = shiftService;
        _dbFactory = dbFactory;
        _parameterService = parameterService;
        _logger = logger;
    }

    public void SetPaymentTypes(BasePaymentType baseType, DerivedPaymentType derivedType)
    {
        _selling.BaseType = baseType;
        _selling.DerivedType = derivedType;
    }

    /// <inheritdoc/>
    public void SetResourceCode(ResourceCode resourceCode)
    {
        _selling.ResourceKey = resourceCode.ResourceKey;
        _selling.ResourceCode = resourceCode.ResourceKey;
        _selling.ResourceName = resourceCode.ResourceName;
        _selling.SellingPrice = resourceCode.ResourcePrice;
    }

    /// <inheritdoc/>
    public void SetAmount(decimal amount)
    {
        _selling.Amount = amount;
    }
    
    /// <inheritdoc/>
    public async Task SetCheckNumber()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var chekNumberSetting = await db.Settings.FindAsync(SettingsKey.Sale);
        if (chekNumberSetting == null)
            return;

        chekNumberSetting.Value++;
        
        _selling.CheckNumber = chekNumberSetting.Value;
        
        db.Update(chekNumberSetting);
        await db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public void SetRequestedVolume(string volume, bool isCost)
    {
        var decimalValue = decimal.Parse(volume, CultureInfo.InvariantCulture);
        
        if (isCost)
        {
            _selling.RequestedCost = Math.Round(decimalValue, 2);
            _selling.RequestedAmount = _selling.RequestedCost / _selling.Amount;
        }
        else
        {
            _selling.RequestedAmount = Math.Round(decimalValue, 3);
            _selling.RequestedCost = _selling.RequestedAmount / _selling.Amount;
        }
    }

    /// <inheritdoc/>
    public void SetPersonKey(int personKey, string? personName)
    {
        _selling.PersonName = personName;
        _selling.PersonKey = personKey;
    }

    /// <inheritdoc/>
    public async Task SetShiftNumber()
    {
        var shift = await _shiftService.GetOpenedShiftOrDefaultAsync();
        _selling.ShiftKey = shift!.ShiftKey;
    }

    /// <inheritdoc/>
    public async Task SetTerminalNumber()
    {
        try
        {
            var terminalNumber = await _parameterService.GetValueAsync(AppParameter.SerialNO111);
            _selling.TerminalKey = Convert.ToInt64(terminalNumber);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }

    /// <inheritdoc/>
    public async Task SetIssuerNumber()
    {
        try
        {
            if (_selling.DerivedType == DerivedPaymentType.FuelCard) //TODO: добавить логику считывания эмитента из топливной карты
                return;
            
            var issuerNumber = await _parameterService.GetValueAsync(AppParameter.IssuerId);
            _selling.IssuerCardId = Convert.ToInt32(issuerNumber);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }

    /// <inheritdoc/>
    public Selling Build()
    {
        _selling.TransactionDatetime = DateTime.Now;
        _selling.ShopCost = _selling.SellingPrice * _selling.Amount; //TODO: тут расчёт скидок.
        
        return _selling;
    }
}