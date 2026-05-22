using Microsoft.EntityFrameworkCore;
using Terminal.Core.DbEntities;
using Terminal.Core.DbEntities.MainDb;
using Terminal.Core.Models;

namespace Terminal.Persistence.MainDB;

/// <summary>
/// Контекст БД терминала.
/// </summary>
public partial class DataContext : DbContext
{
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public DataContext()
    {
    }

    /// <summary>
    /// Конструктор с параметрами работы.
    /// </summary>
    /// <param name="options">Параметры.</param>
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    
    /// <summary>
    /// Конфигурирование контекста.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = GetDefaultDbPath();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    /// <inheritdoc cref="Allow"/>
    public virtual DbSet<Allow> Allows { get; set; }

    /// <inheritdoc cref="BonusChange"/>
    public virtual DbSet<BonusChange> BonusChanges { get; set; }

    /// <inheritdoc cref="CardPassword"/>
    public virtual DbSet<CardPassword> CardPasswords { get; set; }

    /// <inheritdoc cref="CardUpdate"/>
    public virtual DbSet<CardUpdate> CardUpdates { get; set; }

    /// <inheritdoc cref="Correction"/>
    public virtual DbSet<Correction> Corrections { get; set; }

    /// <inheritdoc cref="Dispenser"/>
    public virtual DbSet<Dispenser> Dispensers { get; set; }

    /// <inheritdoc cref="Event"/>
    public virtual DbSet<Event> Events { get; set; }

    /// <inheritdoc cref="IssuerFuelTable"/>
    public virtual DbSet<IssuerFuelTable> IssuerFuelTables { get; set; }

    /// <inheritdoc cref="ListOrg"/>
    public virtual DbSet<ListOrg> ListOrgs { get; set; }

    /// <inheritdoc cref="ListOwner"/>
    public virtual DbSet<ListOwner> ListOwners { get; set; }

    /// <inheritdoc cref="Payment"/>
    public virtual DbSet<Payment> Payments { get; set; }

    /// <inheritdoc cref="PosUpdate"/>
    public virtual DbSet<PosUpdate> PosUpdates { get; set; }

    /// <inheritdoc cref="Prohibition"/>
    public virtual DbSet<Prohibition> Prohibitions { get; set; }

    /// <inheritdoc cref="Repayment"/>
    public virtual DbSet<Repayment> Repayments { get; set; }

    /// <inheritdoc cref="Request"/>
    public virtual DbSet<Request> Requests { get; set; }

    /// <inheritdoc cref="ResourceCode"/>
    public virtual DbSet<ResourceCode> ResourceCodes { get; set; }

    /// <inheritdoc cref="Selling"/>
    public virtual DbSet<Selling> Sales { get; set; }

    /// <inheritdoc cref="SellingIgnore"/>
    public virtual DbSet<SellingIgnore> SellingIgnores { get; set; }

    /// <inheritdoc cref="SellingCoupon"/>
    public virtual DbSet<SellingCoupon> SellingCoupons { get; set; }

    /// <inheritdoc cref="Setting"/>
    public virtual DbSet<Setting> Settings { get; set; }

    /// <inheritdoc cref="Shift"/>
    public virtual DbSet<Shift> Shifts { get; set; }

    /// <inheritdoc cref="TransferCard"/>
    public virtual DbSet<TransferCard> TransferCards { get; set; }

    /// <inheritdoc cref="User"/>
    public virtual DbSet<User> Users { get; set; }
    
    /// <inheritdoc cref="SalesReportResult"/>
    public DbSet<SalesReportResult> SalesReportResults { get; set; }
    
    /// <summary>
    /// Конфигурирование маппинга моделей в таблицы.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
     
        modelBuilder.Entity<SalesReportResult>()
            .HasNoKey()
            .ToView(null);
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    
    /// <summary>
    /// Получить строку подключения к terminal.db.
    /// </summary>
    /// <returns>Строка подключения к БД</returns>
    public static string GetDefaultDbPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbFolder = Path.Combine(appData, "Terminal");
        Directory.CreateDirectory(dbFolder);
        var fullPath = Path.Combine(dbFolder, "terminal.db");

        return fullPath;
    }

    /// <summary>
    /// Получить строку подключения к terminal.db при интеграционном тестировании.
    /// </summary>
    /// <returns>Строка подключения к БД</returns>
    public static string GetTestingDbPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbFolder = Path.Combine(appData, "IntegrationTests", "Terminal");
        Directory.CreateDirectory(dbFolder);
        var fullPath = Path.Combine(dbFolder, "terminal.db");
        
        return fullPath;
    }
}
