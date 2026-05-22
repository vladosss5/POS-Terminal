using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Persistence.MainDB.Mappings;

public class IssuerFuelTableMapping : IEntityTypeConfiguration<IssuerFuelTable>
{
    public void Configure(EntityTypeBuilder<IssuerFuelTable> builder)
    {
        builder.HasKey(e => e.IssuerFuelCodeKey);

        builder.ToTable("issuer_fuel_table");

        builder.Property(e => e.IsCard).HasColumnType("TINYINT");
        builder.Property(e => e.IssuerId).HasColumnName("IssuerID");
    }
}