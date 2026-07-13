using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Persistence.MainDB.Mappings;

public class RequestMapping : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(e => e.VendorKey);

        builder.ToTable("request");

        builder.Property(e => e.VendorKey).ValueGeneratedNever();
        builder.Property(e => e.CompleteVolume).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.InitialVolume).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.LastVolume).HasColumnType("NUMERIC( 20, 3 )");
        builder.Property(e => e.ShopCost).HasColumnType("NUMERIC( 20, 3 )");
    }
}