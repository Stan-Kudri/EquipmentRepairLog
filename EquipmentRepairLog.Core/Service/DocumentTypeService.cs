using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;
using System.Data.Entity;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentTypeService(AppDbContext dbContext)
    {
        public void Add(DocumentType documentType)
        {
            ArgumentNullException.ThrowIfNull(documentType);

            if (dbContext.DocumentTypes.FirstOrDefault(e => e.Abbreviation == documentType.Abbreviation
                                                        || e.Name == documentType.Name
                                                        || e.ExecutiveRepairDocNumber == documentType.ExecutiveRepairDocNumber) != null)
            {
                throw new ArgumentException("Data already in use.", nameof(documentType));
            }

            dbContext.DocumentTypes.Add(documentType);
            dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = dbContext.DocumentTypes.FirstOrDefault(e => e.Id == id)
                        ?? throw new InvalidOperationException("Interaction element not found.");

            dbContext.DocumentTypes.Remove(item);
            dbContext.SaveChanges();
        }

        public DocumentType? GetDocumentType(Guid id)
            => dbContext.DocumentTypes.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public DocumentType? GetDocumentType(string abbreviation)
            => dbContext.DocumentTypes.AsNoTracking().FirstOrDefault(e => e.Abbreviation == abbreviation);
    }
}
