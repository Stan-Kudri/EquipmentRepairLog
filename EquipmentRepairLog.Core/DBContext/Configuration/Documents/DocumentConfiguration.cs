using EquipmentRepairLog.Core.Data.DocumentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration.Documents
{
    public class DocumentConfiguration : EntityBaseConfiguration<Document>
    {
        protected override void ConfigureModel(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("document");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.RegistrationDate).IsRequired().HasColumnType("DATETIME").HasColumnName("date_registration").HasDefaultValue(DateTime.Now);
            builder.Property(x => x.RepairDate).IsRequired().HasColumnName("repair_date").HasColumnType("DATETIME");
            builder.Property(x => x.ChangeDateRegistrNumber).HasColumnName("change_date_registration").HasColumnType("DATETIME").HasDefaultValue(null);
            builder.Property(x => x.OrdinalNumber).IsRequired().HasColumnName("ordinal_number").HasColumnType("INTEGER");
            builder.Property(x => x.RegistrationNumber).IsRequired().HasColumnName("registration_numer").HasMaxLength(128);
            builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(256);
            builder.HasMany(x => x.Documents).WithMany().UsingEntity(x => x.ToTable("execut_repair_documentation"));
        }
    }
}
