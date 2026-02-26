using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Terminal.Core.DbEntities;

namespace Terminal.Data.Mappings;

public class SettingMapping : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(e => e.SettingsKey);

        builder.ToTable("settings");

        builder.Property(e => e.SettingsKey).ValueGeneratedNever();
    }
}