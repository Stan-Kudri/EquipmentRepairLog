using EquipmentRepairLog.Core.Data.EquipmentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration.Equipments
{
    public class EquipmentConfiguration : EntityBaseConfiguration<Equipment>
    {
        protected override void ConfigureModel(EntityTypeBuilder<Equipment> builder)
        {
            builder.ToTable("equipment");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(256).HasDefaultValue(string.Empty);
            builder.HasMany(x => x.EquipmentsKKS).WithOne(x => x.Equipment).HasForeignKey(x => x.EquipmentId).IsRequired();
            builder.HasMany(x => x.EquipmentTypes).WithOne(x => x.Equipment).HasForeignKey(x => x.EquipmentId).IsRequired();
        }
    }
}
