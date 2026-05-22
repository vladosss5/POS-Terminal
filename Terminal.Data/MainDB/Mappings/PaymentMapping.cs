using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class PaymentMapping : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(e => e.PaymentShopKey);

        builder.ToTable("payment");

        builder.HasIndex(e => e.Guid, "PaymentIndex").IsUnique();

        builder.Property(e => e.AppValue).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
        builder.Property(e => e.CommonApplicationId).HasColumnName("CommonApplicationID");
        builder.Property(e => e.ElectronicNumber).HasColumnType("BIGINT");
        builder.Property(e => e.GraphicalNumber).HasColumnType("NUMERIC( 20 )");
        builder.Property(e => e.Guid).HasColumnType("VARCHAR(35)");
        builder.Property(e => e.IsSent).HasColumnType("BOOLEAN");
        builder.Property(e => e.Nz)
            .HasColumnType("VARCHAR(255)")
            .HasColumnName("NZ");
        builder.Property(e => e.PaymentDate).HasColumnType("DATETIME");
        builder.Property(e => e.PaymentSum).HasColumnType("NUMERIC(20,3)");
        builder.Property(e => e.PaymentVolume).HasColumnType("NUMERIC(20,3)");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
    }
}