using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class AllowMapping : IEntityTypeConfiguration<Allow>
{
    public void Configure(EntityTypeBuilder<Allow> builder)
    {
        builder.HasKey(e => e.AllowKey);

        builder.ToTable("allow");

        builder.Property(e => e.RequestServer).HasColumnType("VARCHAR(255)");
    }
}