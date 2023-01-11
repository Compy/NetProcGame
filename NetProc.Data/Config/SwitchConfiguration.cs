using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetProc;

namespace NetProc.Data.Config
{
    public class SwitchConfiguration : IEntityTypeConfiguration<SwitchConfigFileEntry>
    {
        public void Configure(EntityTypeBuilder<SwitchConfigFileEntry> builder)
        {
            builder.HasKey(k => k.Number);
            builder.Property(b => b.Name).IsRequired();
            builder.ToTable("Switches");
        }
    }
}
