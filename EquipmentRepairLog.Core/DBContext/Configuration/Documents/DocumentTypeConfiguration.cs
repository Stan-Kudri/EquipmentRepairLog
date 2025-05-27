using EquipmentRepairLog.Core.Data.StandardModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration.Documents
{
    public class DocumentTypeConfiguration : EntityBaseConfiguration<DocumentType>
    {
        protected override void ConfigureModel(EntityTypeBuilder<DocumentType> builder)
        {
            builder.ToTable("document_type");
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            builder.HasIndex(x => x.Abbreviation).IsUnique();
            builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
            builder.Property(x => x.IsOnlyTypeDocInRepairLog).HasColumnName("is_single_type_document").IsRequired().HasDefaultValue(false);
            builder.HasMany(x => x.Documents).WithOne(x => x.DocumentType).HasForeignKey(x => x.DocumentTypeId).IsRequired();
        }
    }
}
