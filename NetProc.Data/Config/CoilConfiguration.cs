using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetProc.Data.Config
{
    public class CoilConfiguration : IEntityTypeConfiguration<CoilConfigFileEntry>
    {
        public void Configure(EntityTypeBuilder<CoilConfigFileEntry> builder)
        {
            builder.HasKey(k => k.Number);
            builder.Property(b => b.Name).IsRequired();
            builder.ToTable("Coils");
        }
    }
}
