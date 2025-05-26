using EquipmentRepairLog.Core.Data.StandardModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration
{
    public class DivisionConfiguration : EntityBaseConfiguration<Division>
    {
        protected override void ConfigureModel(EntityTypeBuilder<Division> builder)
        {
            builder.ToTable("division");
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
            builder.Property(x => x.Number).HasColumnName("number").IsRequired();
            builder.HasMany(x => x.Documents).WithOne(x => x.Division).HasForeignKey(x => x.DivisionId).IsRequired();
        }
    }
}
