using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Terminal.Core.DbEntities;
using Terminal.Data;

namespace Terminal.Data.Context;

public partial class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = GetDefaultDbPath();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    public virtual DbSet<ResourceCode> ResourceCode { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResourceCode>(entity =>
        {
            entity.ToTable("resource_code");
            
            entity.HasKey(e => e.FuelCodeKey);
            
            entity.Property(e => e.FuelCodeKey).ValueGeneratedOnAdd();
            entity.Property(e => e.IsShow).HasColumnType("tinyint");
            entity.Property(e => e.Density).HasColumnType("NUMERIC(20,4)");
            entity.Property(e => e.Temperature).HasColumnType("NUMERIC(20,4)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    
    public static string GetDefaultDbPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbFolder = Path.Combine(appData, "Terminal");
        Directory.CreateDirectory(dbFolder);
        var fullPath = Path.Combine(dbFolder, "terminal.db");

        return fullPath;
    }
}
