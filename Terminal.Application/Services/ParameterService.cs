using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.DbEntities.ParamDb;
using Terminal.Core.Enums;
using Terminal.Core.IRepositories;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class ParameterService : IParameterService
{
    /// <inheritdoc cref="IParamRepository" />
    private readonly IParamRepository _paramRepository;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ParameterService(IParamRepository paramRepository)
    {
        _paramRepository = paramRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckSetupComplete()
    {
        var installedParameter = await _paramRepository.GetByNameAsync(AppParameter.IsInstalled);
        return installedParameter is { Value: "1" };
    }

    /// <inheritdoc/>
    public async Task<string?> GetValueAsync(AppParameter parameterName)
    {
        var parameter = await _paramRepository.GetByNameAsync(parameterName);
        
        return parameter?.Value;
    }

    /// <inheritdoc/>
    public async Task SetValueAsync(AppParameter parameterName, string value)
    {
        var parameter = await _paramRepository.GetByNameAsync(parameterName);

        if (parameter == null)
        {
            parameter = new Param
            {
                Name = parameterName.ToString(),
                Value = value
            };

            await _paramRepository.AddAsync(parameter);
        }
        else
        {
            parameter.Value = value;
            await _paramRepository.UpdateAsync(parameter);
        }
    }
}