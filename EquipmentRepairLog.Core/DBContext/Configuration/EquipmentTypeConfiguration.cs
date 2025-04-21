using EquipmentRepairLog.Core.Data.EquipmentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration
{
    public class EquipmentTypeConfiguration : EntityBaseConfiguration<EquipmentType>
    {
        protected override void ConfigureModel(EntityTypeBuilder<EquipmentType> builder)
        {
            builder.ToTable("equipment_type");
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
        }
    }
}
