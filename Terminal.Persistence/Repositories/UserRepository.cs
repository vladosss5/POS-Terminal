using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.MainDb;
using Terminal.Core.IRepositories;
using Terminal.Persistence.MainDB;

namespace Terminal.Persistence.Repositories;

/// <inheritdoc/>
public class UserRepository : IUserRepository
{
    /// <inheritdoc cref="DataContext" />
    private readonly DataContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public UserRepository(IDbContextFactory<DataContext> mainDbFactory)
    {
        _context = mainDbFactory.CreateDbContext();
    }
    
    /// <inheritdoc/>
    public async Task<User?> GetByUserName(string name)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Name == name);
    }
}