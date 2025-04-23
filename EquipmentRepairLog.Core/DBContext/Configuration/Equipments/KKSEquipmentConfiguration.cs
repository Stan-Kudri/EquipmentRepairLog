using EquipmentRepairLog.Core.Data.EquipmentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration.Equipments
{
    public class KKSEquipmentConfiguration : EntityBaseConfiguration<KKSEquipment>
    {
        protected override void ConfigureModel(EntityTypeBuilder<KKSEquipment> builder)
        {
            builder.ToTable("kks_equipment");
            builder.Property(x => x.KKS).HasColumnName("kks").IsRequired().HasMaxLength(128);
            builder.HasOne(x => x.Equipment).WithMany(x => x.EquipmentsKKS);
            builder.HasMany(x => x.KKSEquipmentDocuments).WithMany(x => x.KKSEquipment).UsingEntity(e => e.ToTable("equipment_KKS_documents"));
        }
    }
}
