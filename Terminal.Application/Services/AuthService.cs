using Terminal.Application.Interfaces.Services;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.Interfaces;
using Terminal.Core.IRepositories;

namespace Terminal.Application.Services;

/// <inheritdoc/>
public class AuthService : IAuthService
{
    /// <inheritdoc cref="ICryptographyService"/>
    private readonly ICryptographyService _cryptographyService;

    private readonly IUserRepository _userRepository;
    
    public User? CurrentUser { get; private set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AuthService(
        ICryptographyService cryptographyService, 
        IUserRepository userRepository)
    {
        _cryptographyService = cryptographyService;
        _userRepository = userRepository;
    }
    
    /// <inheritdoc/>
    public async Task<bool> LoginWithPasswordAsync(string userName, string password)
    {
        var user = await _userRepository.GetByUserName(userName);

        if (user == null)
            return false;
        
        if (!_cryptographyService.VerifyPasswordWithMd5(password, user.UserPassword!))
            return false;

        CurrentUser = user;

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> LoginWithCardNumber(string userName, int cardNumber)
    {
        var user = await _userRepository.GetByUserName(userName);

        if (user == null)
            return false;
        
        if (user.CardNumber != cardNumber)
            return false;

        CurrentUser = user;

        return true;
    }

    /// <inheritdoc/>
    public bool AuthenticateOperator(string password) 
        => CurrentUser != null && _cryptographyService.VerifyPasswordWithMd5(password, CurrentUser.UserPassword!);

    /// <inheritdoc/>
    public void Logout() => CurrentUser = null;
}