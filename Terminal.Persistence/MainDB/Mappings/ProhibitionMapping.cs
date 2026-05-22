using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Persistence.MainDB.Mappings;

public class ProhibitionMapping : IEntityTypeConfiguration<Prohibition>
{
    public void Configure(EntityTypeBuilder<Prohibition> builder)
    {
        builder.HasKey(e => e.ProhibitionKey);

        builder.ToTable("prohibition");

        builder.Property(e => e.BeginDate).HasColumnType("DATETIME");
        builder.Property(e => e.EndDate).HasColumnType("DATETIME");
        builder.Property(e => e.Sign).HasColumnType("TINYINT");
    }
} 