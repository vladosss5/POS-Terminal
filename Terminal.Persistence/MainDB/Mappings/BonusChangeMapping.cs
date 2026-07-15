using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Persistence.MainDB.Mappings;

public class BonusChangeMapping : IEntityTypeConfiguration<BonusChange>
{
    public void Configure(EntityTypeBuilder<BonusChange> builder)
    {
        builder.HasKey(e => e.BonusChangeShopKey);

        builder.ToTable("bonus_change");

        builder.HasIndex(e => e.SetOfGoodsGuid, "BonChange1").IsUnique();

        builder.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
        builder.Property(e => e.BonusChange1)
            .HasColumnType("NUMERIC( 20, 3 )")
            .HasColumnName("BonusChange");
        builder.Property(e => e.CommodityGuid).HasColumnType("VARCHAR( 255 )");
        builder.Property(e => e.ElectronicNumber).HasColumnType("NUMERIC( 20 )");
        builder.Property(e => e.GraphicalNumber).HasColumnType("NUMERIC( 20 )");
        builder.Property(e => e.IssuerCardId).HasColumnName("IssuerCardID");
        builder.Property(e => e.IssuerTerminalId).HasColumnName("IssuerTerminalID");
        builder.Property(e => e.SetOfGoodsGuid).HasColumnType("VARCHAR( 255 )");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
        builder.Property(e => e.TransactionDatetime).HasColumnType("DATETIME");
    }
}