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

    public virtual DbSet<Allow> Allows { get; set; }

    public virtual DbSet<BonusChange> BonusChanges { get; set; }

    public virtual DbSet<CardPassword> CardPasswords { get; set; }

    public virtual DbSet<CardUpdate> CardUpdates { get; set; }

    public virtual DbSet<Correction> Corrections { get; set; }

    public virtual DbSet<Dispenser> Dispensers { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<IssuerFuelTable> IssuerFuelTables { get; set; }

    public virtual DbSet<ListOrg> ListOrgs { get; set; }

    public virtual DbSet<ListOwner> ListOwners { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PosUpdate> PosUpdates { get; set; }

    public virtual DbSet<Prohibition> Prohibitions { get; set; }

    public virtual DbSet<Repayment> Repayments { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<ResourceCode> ResourceCodes { get; set; }

    public virtual DbSet<Selling> Sellings { get; set; }

    public virtual DbSet<SellingIgnore> SellingIgnores { get; set; }

    public virtual DbSet<SellingCoupon> SellingCoupons { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<TransferCard> TransferCards { get; set; }

    public virtual DbSet<User> Users { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
