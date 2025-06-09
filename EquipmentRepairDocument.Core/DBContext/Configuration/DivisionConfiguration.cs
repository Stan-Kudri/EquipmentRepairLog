using EquipmentRepairDocument.Core.Data.StandardModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairDocument.Core.DBContext.Configuration
{
    public class DivisionConfiguration : EntityBaseConfiguration<Division>
    {
        protected override void ConfigureModel(EntityTypeBuilder<Division> builder)
        {
            builder.ToTable("division");
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            builder.HasIndex(x => x.Abbreviation).IsUnique();
            builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
            builder.HasIndex(x => x.Number).IsUnique();
            builder.Property(x => x.Number).HasColumnName("number").IsRequired();
            builder.HasMany(x => x.Documents).WithOne(x => x.Division).HasForeignKey(x => x.DivisionId).IsRequired();
        }
    }
}
