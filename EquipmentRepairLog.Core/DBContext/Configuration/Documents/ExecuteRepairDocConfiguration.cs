using EquipmentRepairLog.Core.Data.DocumentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentRepairLog.Core.DBContext.Configuration.Documents
{
    public class ExecuteRepairDocConfiguration : EntityBaseConfiguration<ExecuteRepairDocument>
    {
        protected override void ConfigureModel(EntityTypeBuilder<ExecuteRepairDocument> builder)
        {
            builder.ToTable("execute_repair_documents");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
        }
    }
}
