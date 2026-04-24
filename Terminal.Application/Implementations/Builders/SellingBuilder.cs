using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.Builders;
using Terminal.Application.Interfaces.DbEntitiesServices;
using Terminal.Core.DbEntities;
using Terminal.Core.Enums;
using Terminal.Data.Context;

namespace Terminal.Application.Implementations.Builders;

/// <inheritdoc/>
public class SellingBuilder : ISellingBuilder
{
    /// <inheritdoc cref="Selling" />
    private readonly Selling _selling = new();

    /// <inheritdoc cref="IShiftService" />
    private readonly IShiftService _shiftService;
    
    /// Фабрика создающая <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;
    
    /// Фабрика экземпляров: <inheritdoc cref="ParamDbContext"/>
    private readonly IDbContextFactory<ParamDbContext> _paramDbFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public SellingBuilder(
        IShiftService shiftService,
        IDbContextFactory<DataContext> dbFactory, 
        IDbContextFactory<ParamDbContext> paramDbFactory)
    {
        _shiftService = shiftService;
        _dbFactory = dbFactory;
        _paramDbFactory = paramDbFactory;
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
        var decimalValue = decimal.Parse(volume);
        
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
        await using var paramDb = await _paramDbFactory.CreateDbContextAsync();
        var terminalNumber = await paramDb.Params.FirstOrDefaultAsync(x => x.Name == "SerialNO111");
        _selling.TerminalKey = Convert.ToInt64(terminalNumber!.Value);
    }

    /// <inheritdoc/>
    public async Task SetIssuerNumber()
    {
        if (_selling.DerivedType == DerivedPaymentType.FuelCard) //TODO: добавить логику считывания эмитента из топливной карты
            return;
        
        await using var paramDb = await _paramDbFactory.CreateDbContextAsync();
        var issuerNumber = await paramDb.Params.FirstOrDefaultAsync(x => x.Name == "IssuerId");
        _selling.IssuerCardId = Convert.ToInt32(issuerNumber!.Value);
    }

    /// <inheritdoc/>
    public Selling Build()
    {
        _selling.TransactionDatetime = DateTime.Now;
        _selling.ShopCost = _selling.SellingPrice * _selling.Amount; //TODO: тут расчёт скидок.
        
        return _selling;
    }
}