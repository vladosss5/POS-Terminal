using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;

namespace Terminal.Data.Mappings;

public class SellingCouponMapping : IEntityTypeConfiguration<SellingCoupon>
{
    public void Configure(EntityTypeBuilder<SellingCoupon> builder)
    {
        builder.HasKey(e => e.SellingCouponShopKey);

        builder.ToTable("sellingcoupon");

        builder.Property(e => e.BaseType).HasColumnType("INTEGER (1)");
        builder.Property(e => e.CommodityGuid).HasColumnType("VARCHAR (255)");
        builder.Property(e => e.DerivedType).HasColumnType("INTEGER (1)");
        builder.Property(e => e.ElectronicNumber).HasColumnType("BIGINT");
        builder.Property(e => e.GraphicalNumber).HasColumnType("VARCHAR (250)");
        builder.Property(e => e.SetOfGoodsGuid).HasColumnType("VARCHAR (255)");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC (14)");
        builder.Property(e => e.TransactionDatetime).HasColumnType("DATETIME");
        builder.Property(e => e.UsedVolume).HasColumnType("NUMERIC (20, 3)");
    }
}