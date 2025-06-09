using EquipmentRepairDocument.Core.Data.StandardModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairDocument.Core.DBContext.Configuration
{
    public class PerfomerConfiguration : EntityBaseConfiguration<Perfomer>
    {
        protected override void ConfigureModel(EntityTypeBuilder<Perfomer> builder)
        {
            builder.ToTable("perfomer");
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            builder.HasIndex(x => x.Abbreviation).IsUnique();
            builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
            builder.HasMany(x => x.Documents).WithMany(x => x.Perfomers).UsingEntity(e => e.ToTable("perfomer_work_document"));
        }
    }
}
