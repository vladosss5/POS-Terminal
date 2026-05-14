using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class CardUpdateMapping : IEntityTypeConfiguration<CardUpdate>
{
    public void Configure(EntityTypeBuilder<CardUpdate> builder)
    {
        builder.HasKey(e => e.CardUpdateKey);

        builder.ToTable("card_update");

        builder.Property(e => e.AfterValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.ApplicationType).HasColumnType("TINYINT");
        builder.Property(e => e.BeforeValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.CorrectionType).HasColumnType("TINYINT");
        builder.Property(e => e.ElectronicNumber).HasColumnType("BIGINT");
        builder.Property(e => e.EndDate).HasColumnType("DATETIME");
        builder.Property(e => e.EnterDate).HasColumnType("DATETIME");
        builder.Property(e => e.IsSent).HasColumnType("BOOLEAN");
        builder.Property(e => e.ParameterAddValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.ParameterRepValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.ParameterType).HasColumnType("TINYINT");
        builder.Property(e => e.ResultCode).HasColumnType("TINYINT");
        builder.Property(e => e.StartDate).HasColumnType("DATETIME");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
        builder.Property(e => e.UpdateDate).HasColumnType("DATETIME");
    }
}