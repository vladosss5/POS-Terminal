using Terminal.Core.Enums;

namespace Terminal.Application.Interfaces.Services;

public interface IParameterService
{
    public Task<string> GetValue(AppParameter parameterName);
    
    public Task SetValue(AppParameter parameterName, string value);
}