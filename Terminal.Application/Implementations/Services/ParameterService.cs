using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.Enums;
using Terminal.Data.Context;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class ParameterService : IParameterService
{
    /// Фабрика <inheritdoc cref="ParamDbContext" />
    private readonly IDbContextFactory<ParamDbContext> _dbContextFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public ParameterService(IDbContextFactory<ParamDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc/>
    public async Task<string> GetValueAsync(AppParameter parameterName)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var parameter = await db.Params.FirstOrDefaultAsync(x => x.Name == parameterName.ToString());
       
        return parameter == null 
            ? throw new Exception("Параметр не найден") 
            : parameter.Value;
    }

    /// <inheritdoc/>
    public async Task SetValueAsync(AppParameter parameterName, string value)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        
        var parameter = await db.Params.FirstOrDefaultAsync(x => x.Name == parameterName.ToString());

        if (parameter == null)
            throw new Exception("Параметр не найден");

        parameter.Value = value;

        db.Update(parameter);
        await db.SaveChangesAsync();
    }
}