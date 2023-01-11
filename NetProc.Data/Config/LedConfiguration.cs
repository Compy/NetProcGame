using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetProc;

namespace NetProc.Data.Config
{
    public class LedConfiguration : IEntityTypeConfiguration<LampConfigFileEntry>
    {
        public void Configure(EntityTypeBuilder<LampConfigFileEntry> builder)
        {
            builder.HasKey(k => k.Number);
            builder.Property(b => b.Name).IsRequired();
            builder.ToTable("Leds");
        }
    }
}
