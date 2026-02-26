using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;

namespace Terminal.Data.Mappings;

public class CorrectionMapping : IEntityTypeConfiguration<Correction>
{
    public void Configure(EntityTypeBuilder<Correction> builder)
    {
        builder.HasKey(e => e.CorrectionsKey);

        builder.ToTable("corrections");

        builder.Property(e => e.ApplicationType).HasColumnType("TINYINT");
        builder.Property(e => e.CorrectionType).HasColumnType("TINYINT");
        builder.Property(e => e.ElectronicNumber).HasColumnType("BIGINT");
        builder.Property(e => e.EndDate).HasColumnType("DATETIME");
        builder.Property(e => e.EnterDate).HasColumnType("DATETIME");
        builder.Property(e => e.IsDelete).HasColumnType("BOOL");
        builder.Property(e => e.Note).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.ParameterAddValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.ParameterRepValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.ParameterType).HasColumnType("TINYINT");
        builder.Property(e => e.StartDate).HasColumnType("DATETIME");
    }
}