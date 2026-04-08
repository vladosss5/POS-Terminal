using Microsoft.EntityFrameworkCore;
using Terminal.Core.ParamDbEntities;

namespace Terminal.Data.Context;

/// <summary>
/// Контекст БД параметров.
/// </summary>
public partial class ParamDbContext : DbContext
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ParamDbContext()
    { }
    
    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    public ParamDbContext(DbContextOptions<ParamDbContext> options) : base(options)
    { }
    
    /// <summary>
    /// Конфигурирование контекста.
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) 
            return;
        
        var dbPath = GetDefaultDbPath();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
    
    
    /// <inheritdoc cref="Param"/>
    public virtual DbSet<Param> Params { get; set; }

    
    /// <summary>
    /// Конфигурирование маппинга моделей в таблицы.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Param>(entity =>
        {
            entity.ToTable("param");

            entity.HasKey(e => e.Name);
            
            entity
                .Property(e => e.Name)
                .HasMaxLength(200);
            
            entity
                .Property(e => e.Value)
                .HasMaxLength(1000)
                .IsRequired();
        });
        
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
        var fullPath = Path.Combine(dbFolder, "param.db");

        return fullPath;
    }
}