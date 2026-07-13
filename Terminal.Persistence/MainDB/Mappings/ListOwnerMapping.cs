using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.Entities.DbEntities.MainDb;

namespace Terminal.Persistence.MainDB.Mappings;

public class ListOwnerMapping : IEntityTypeConfiguration<ListOwner>
{
    public void Configure(EntityTypeBuilder<ListOwner> builder)
    {
        builder.HasKey(e => e.ListOwnerKey);

        builder.ToTable("list_owner");

        builder.Property(e => e.GraphicalNumber).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.OwnerName).HasColumnType("VARCHAR(255)");
    }
}