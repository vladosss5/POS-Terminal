using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class RepaymentMapping : IEntityTypeConfiguration<Repayment>
{
    public void Configure(EntityTypeBuilder<Repayment> builder)
    {
        builder.HasKey(e => e.RepaymentShopKey);

        builder.ToTable("repayment");

        builder.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
        builder.Property(e => e.CardType).HasColumnType("TINYINT");
        builder.Property(e => e.ElectronicNumber).HasColumnType("BIGINT");
        builder.Property(e => e.IsSent).HasColumnType("BOOLEAN");
        builder.Property(e => e.RepaymentDate).HasColumnType("DATETIME");
        builder.Property(e => e.RepaymentType).HasColumnType("TINYINT");
        builder.Property(e => e.RepaymentValue).HasColumnType("NUMERIC(20,3)");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
    }
}