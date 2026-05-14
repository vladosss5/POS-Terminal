using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities.MainDb;

namespace Terminal.Data.MainDB.Mappings;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(e => e.UserId).ValueGeneratedNever();
        builder.Property(e => e.CardNumber).HasColumnType("NUMERIC( 16 )");
        builder.Property(e => e.EcardNumber)
            .HasColumnType("NUMERIC( 16 )")
            .HasColumnName("ECardNumber");
        builder.Property(e => e.Name).HasColumnType("VARCHAR( 50 )");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
        builder.Property(e => e.UserPassword).HasColumnType("VARCHAR( 35 )");
        builder.Property(e => e.UserType).HasColumnType("NUMERIC( 2 )");
    }
}