using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class ShiftMapping : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(e => e.ShiftShopKey);

        builder.ToTable("shift");

        builder.Property(e => e.IsOpened).HasColumnType("BOOLEAN");
        builder.Property(e => e.ShiftDate).HasColumnType("DATETIME");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
    }
}