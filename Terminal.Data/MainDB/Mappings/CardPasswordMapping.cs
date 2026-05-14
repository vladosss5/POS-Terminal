using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class CardPasswordMapping : IEntityTypeConfiguration<CardPassword>
{
    public void Configure(EntityTypeBuilder<CardPassword> builder)
    {
        builder.HasKey(e => e.CardPasswordKey);

        builder.ToTable("card_password");

        builder.Property(e => e.ElectronicNumber).HasColumnType("NUMERIC( 20 )");
        builder.Property(e => e.GraphicalNumber).HasColumnType("NUMERIC( 20 )");
        builder.Property(e => e.GraphicalNumberUpos)
            .HasColumnType("NUMERIC( 20 )")
            .HasColumnName("GraphicalNumberUPOS");
        builder.Property(e => e.LastSessionEnd).HasColumnType("BIGINT");
        builder.Property(e => e.LastSessionStart).HasColumnType("BIGINT");
    }
}