using EquipmentRepairLog.Core.Data.StandardModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration
{
    public class RepairFacilityConfiguration : EntityBaseConfiguration<RepairFacility>
    {
        protected override void ConfigureModel(EntityTypeBuilder<RepairFacility> builder)
        {
            builder.ToTable("repair_facility");
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
            builder.Property(x => x.Number).HasColumnName("number").IsRequired().HasColumnType("INTEGER");
            builder.HasMany(x => x.Documents).WithOne(x => x.RepairFacility).HasForeignKey(x => x.RepairFacilityId).IsRequired();
        }
    }
}
