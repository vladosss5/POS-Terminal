using Microsoft.EntityFrameworkCore;
using Terminal.Application.Interfaces.Services;
using Terminal.Core.DbEntities;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Persistence.MainDB;

namespace Terminal.Application.Implementations.Services;

/// <inheritdoc/>
public class AuthService : IAuthService
{
    /// <inheritdoc cref="ICryptographyService"/>
    private readonly ICryptographyService _cryptographyService;

    /// Фабрика экземпляров: <inheritdoc cref="DataContext"/>
    private readonly IDbContextFactory<DataContext> _dbFactory;
    
    public User? CurrentUser { get; private set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public AuthService(
        ICryptographyService cryptographyService, 
        IDbContextFactory<DataContext> dbFactory)
    {
        _cryptographyService = cryptographyService;
        _dbFactory = dbFactory;
    }
    
    /// <inheritdoc/>
    public async Task<bool> LoginWithPasswordAsync(string userName, string password)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(x => x.Name == userName);

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
        await using var db = await _dbFactory.CreateDbContextAsync();

        var user = await db.Users.FirstOrDefaultAsync(x => x.Name == userName);

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