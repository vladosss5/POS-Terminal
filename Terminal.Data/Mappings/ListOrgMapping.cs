using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;

namespace Terminal.Data.Mappings;

public class ListOrgMapping : IEntityTypeConfiguration<ListOrg>
{
    public void Configure(EntityTypeBuilder<ListOrg> builder)
    {
        builder.HasKey(e => e.ListOrgKey);

        builder.ToTable("list_org");

        builder.Property(e => e.OrganisationName).HasColumnType("VARCHAR(255)");
    }
}