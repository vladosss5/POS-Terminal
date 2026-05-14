using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class TransferCardMapping : IEntityTypeConfiguration<TransferCard>
{
    public void Configure(EntityTypeBuilder<TransferCard> builder)
    {
        builder.HasKey(e => e.TransferCardKey);

        builder.ToTable("transfer_card");

        builder.Property(e => e.AppLimit).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.AppSecondLimit).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.AppSecondValue).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.AppValue).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
        builder.Property(e => e.CommonApplicationId).HasColumnName("CommonApplicationID");
        builder.Property(e => e.ElectronicNumber).HasColumnType("NUMERIC( 20 )");
        builder.Property(e => e.GraphicalNumber).HasColumnType("NUMERIC( 20 )");
        builder.Property(e => e.IssuerCardId).HasColumnName("IssuerCardID");
        builder.Property(e => e.ParcelPrice).HasColumnType("NUMERIC( 10, 3 )");
        builder.Property(e => e.ValidityPeriod).HasColumnType("BIGINT");
    }
}