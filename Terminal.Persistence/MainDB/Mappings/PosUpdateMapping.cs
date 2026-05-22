using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Persistence.MainDB.Mappings;

public class PosUpdateMapping : IEntityTypeConfiguration<PosUpdate>
{
    public void Configure(EntityTypeBuilder<PosUpdate> builder)
    {
        builder.HasKey(e => e.PosUpdateShopKey);

        builder.ToTable("pos_update");

        builder.HasIndex(e => e.Guid, "PosUpdateIndex").IsUnique();

        builder.Property(e => e.AfterValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.AppStatus).HasColumnType("TINYINT");
        builder.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
        builder.Property(e => e.BeforeValue).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.ChangeValue).HasColumnType("VARCHAR(35)");
        builder.Property(e => e.ElectronicNumber).HasColumnType("BIGINT");
        builder.Property(e => e.GraphicalNumber).HasColumnType("BIGINT");
        builder.Property(e => e.Guid).HasColumnType("VARCHAR(35)");
        builder.Property(e => e.IsSent).HasColumnType("BOOLEAN");
        builder.Property(e => e.PosUpdateDate).HasColumnType("DATETIME");
        builder.Property(e => e.PosUpdateType).HasColumnType("TINYINT");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
    }
}