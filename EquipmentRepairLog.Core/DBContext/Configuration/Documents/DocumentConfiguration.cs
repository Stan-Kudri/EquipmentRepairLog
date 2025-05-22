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
            builder.Property(x => x.RegistrationDate).IsRequired().HasColumnType("DATETIME").HasColumnName("date_registration").HasDefaultValue(DateTime.Now);
            builder.Property(x => x.RepairDate).IsRequired().HasColumnName("repair_date");
            builder.Property(x => x.ChangeDateRegistrNumber).HasColumnName("change_date_registration").HasDefaultValue(null);
            builder.Property(x => x.OrdinalNumber).IsRequired().HasColumnName("ordinal_number");
            builder.Property(x => x.RegistrationNumber).IsRequired().HasColumnName("registration_numer").HasMaxLength(128);
            builder.HasIndex(x => x.RegistrationNumber).IsUnique();
            builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(256);
            builder.HasMany(x => x.ExecuteRepairDocuments).WithMany(x => x.Documents)
                                                          .UsingEntity<Dictionary<string, object>>(
                                                                        x => x.HasOne<ExecuteRepairDocument>().WithMany().OnDelete(DeleteBehavior.Cascade),
                                                                        x => x.HasOne<Document>().WithMany().OnDelete(DeleteBehavior.Cascade),
                                                                        x => x.ToTable("document_included_execute_repair_document"));
        }
    }
}
