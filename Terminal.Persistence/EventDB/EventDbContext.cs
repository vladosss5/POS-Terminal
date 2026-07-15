using Microsoft.EntityFrameworkCore;
using Terminal.Core.Entities.DbEntities.EventDb;

namespace Terminal.Persistence.EventDB;

/// <summary>
/// Контекст Event.db
/// </summary>
public partial class EventDbContext : DbContext
{
    /// <summary>
    /// Конструктор по-умолчанию.
    /// </summary>
    public EventDbContext()
    {
    }

    /// <summary>
    /// Конструктор с параметрами.
    /// </summary>
    /// <param name="options">Параметры.</param>
    public EventDbContext(DbContextOptions<EventDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Св-во для запросов в таблицу инкассаций.
    /// </summary>
    public virtual DbSet<Incass> Incasses { get; set; }

    /// <summary>
    /// Св-во для запросов в таблицу с логами событий.
    /// </summary>
    public virtual DbSet<ProtocolFilingForm> ProtocolFilingForms { get; set; }


    /// <summary>
    /// Конфигурирование маппинга моделей в таблицы.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Incass>(entity =>
        {
            entity.HasKey(e => e.IncassKey);

            entity.ToTable("incass");

            entity.Property(e => e.LastDatetimeEnd).HasColumnType("DATETIME");
            entity.Property(e => e.LastDatetimeStart).HasColumnType("DATETIME");
        });

        modelBuilder.Entity<ProtocolFilingForm>(entity =>
        {
            entity.HasKey(e => e.ProtokolFillingFormKey);

            entity.ToTable("ProtocolFilingForm");

            entity.Property(e => e.CurrentObjectParameterValue).HasColumnType("DOUBLE");
            entity.Property(e => e.EventDatetime).HasColumnType("DATETIME");
            entity.Property(e => e.EventKey).HasColumnType("NUMERIC(20)");
            entity.Property(e => e.EventValue).HasColumnType("DOUBLE");
            entity.Property(e => e.Hash).HasColumnType("varchar(50)");
            entity.Property(e => e.LatestObjectParameterValue).HasColumnType("DOUBLE");
            entity.Property(e => e.ObjectType).HasColumnType("NUMERIC(20)");
            entity.Property(e => e.SncProjectKey).HasColumnName("sncProjectKey");
            entity.Property(e => e.SubjectType).HasColumnType("NUMERIC(20)");
            
            entity.Property(e => e.ObjectId)
                .HasColumnType("NUMERIC(20)")
                .HasColumnName("ObjectID");
            
            entity.Property(e => e.PlaceId)
                .HasColumnType("NUMERIC(20)")
                .HasColumnName("PlaceID");
            
            entity.Property(e => e.SubjectId)
                .HasColumnType("NUMERIC(20)")
                .HasColumnName("SubjectID");
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
        var fullPath = Path.Combine(dbFolder, "event.db");

        return fullPath;
    }
}
