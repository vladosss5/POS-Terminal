using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;

namespace Terminal.Data.Mappings;

public class SellingIgnoreMapping : IEntityTypeConfiguration<SellingIgnore>
{
    public void Configure(EntityTypeBuilder<SellingIgnore> builder)
    {
        builder.HasKey(e => e.TransactionShopKey);

            builder.ToTable("selling_ignore");

            builder.Property(e => e.Amount).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.AppLimit).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.AppSecondLimit).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.AppSecondValue).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.AppValue).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            builder.Property(e => e.BasePrice).HasColumnType("NUMERIC( 10, 3 )");
            builder.Property(e => e.BaseType).HasColumnType("INTEGER( 1 )");
            builder.Property(e => e.BeginTemperature).HasColumnType("NUMERIC( 10, 4 )");
            builder.Property(e => e.BonusIn).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.BonusInCost).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.BonusOut).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.BonusOutCost).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.ClientCost).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.CommodityGuid).HasColumnType("VARCHAR( 255 )");
            builder.Property(e => e.CommonApplicationId).HasColumnName("CommonApplicationID");
            builder.Property(e => e.Density).HasColumnType("NUMERIC( 10, 4 )");
            builder.Property(e => e.DerivedType).HasColumnType("INTEGER( 1 )");
            builder.Property(e => e.ElectronicNumber).HasColumnType("NUMERIC( 20 )");
            builder.Property(e => e.EndTemperature).HasColumnType("NUMERIC( 10, 4 )");
            builder.Property(e => e.GraphicalNumber).HasColumnType("NUMERIC( 20 )");
            builder.Property(e => e.Guid).HasColumnType("VARCHAR(35)");
            builder.Property(e => e.IssuerCardId).HasColumnName("IssuerCardID");
            builder.Property(e => e.IssuerTerminalId).HasColumnName("IssuerTerminalID");
            builder.Property(e => e.Overflow).HasColumnType("NUMERIC( 10, 4 )");
            builder.Property(e => e.ParcelPrice).HasColumnType("NUMERIC( 10, 3 )");
            builder.Property(e => e.RequestFlags).HasDefaultValue(0);
            builder.Property(e => e.RequestedAmount).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.RequestedCost).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.ResourceName).HasColumnType("VARCHAR( 50 )");
            builder.Property(e => e.SellingFlags).HasColumnType("BIGINT");
            builder.Property(e => e.SellingPrice).HasColumnType("NUMERIC( 10, 3 )");
            builder.Property(e => e.SetOfGoodsGuid).HasColumnType("VARCHAR( 255 )");
            builder.Property(e => e.ShopBaseCost).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.ShopCost).HasColumnType("NUMERIC( 20, 3 )");
            builder.Property(e => e.Sign).HasColumnType("VARCHAR( 255 )");
            builder.Property(e => e.Temperature).HasColumnType("NUMERIC( 10, 4 )");
            builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
            builder.Property(e => e.TransactionDatetime).HasColumnType("DATETIME");
    }
}