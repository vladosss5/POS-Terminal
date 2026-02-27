using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;

namespace Terminal.Data.Mappings;

///<inheritdoc />
public class ResourceCodeMapping : IEntityTypeConfiguration<ResourceCode>
{
    ///<inheritdoc />
    public void Configure(EntityTypeBuilder<ResourceCode> builder)
    {
        builder.HasKey(e => e.FuelCodeKey);

        builder.ToTable("resource_code");

        builder.HasIndex(e => new { e.CollectionKey, e.ResourceKey }, "ResourceUnique").IsUnique();

        builder.Property(e => e.Density).HasColumnType("NUMERIC( 20, 4 )");
        builder.Property(e => e.IsShow).HasColumnType("TINYINT");
        builder.Property(e => e.ResourceName).HasColumnType("VARCHAR( 50 )");
        builder.Property(e => e.ResourcePrice).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.Temperature).HasColumnType("NUMERIC( 20, 4 )");
    }
}