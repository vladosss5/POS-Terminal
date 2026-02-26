using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;

namespace Terminal.Data.Mappings;

public class EventMapping : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.EventsKey);

        builder.ToTable("events");

        builder.Property(e => e.EventDate).HasColumnType("DATETIME");
        builder.Property(e => e.EventInfo).HasColumnType("VARCHAR(255)");
        builder.Property(e => e.TerminalKey).HasColumnType("NUMERIC( 14 )");
    }
}