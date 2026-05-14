using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class DispenserMapping : IEntityTypeConfiguration<Dispenser>
{
    public void Configure(EntityTypeBuilder<Dispenser> builder)
    {
        builder.HasKey(e => e.DispenserShopKey);

        builder.ToTable("dispenser");

        builder.HasIndex(e => new { e.VendorKey, e.ShiftKey, e.TerminalKey }, "BalVender1").IsUnique();

        builder.Property(e => e.BeginBalance).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.BeginBalanceCalculation).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.BeginTemperature).HasColumnType("NUMERIC( 10, 4 )");
        builder.Property(e => e.EndBalance).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.EndBalanceCalculation).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.EndTemperature).HasColumnType("NUMERIC( 10, 4 )");
        builder.Property(e => e.Flags).HasColumnType("BIGINT");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
        builder.Property(e => e.VendorName).HasColumnType("VARCHAR(10)");
    }
}