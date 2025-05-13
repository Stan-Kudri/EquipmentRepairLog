using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.ValidationData;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions.AppException;
using System.Data.Entity;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentTypeService(AppDbContext dbContext, DocumentTypeFactory documentTypeFactory)
    {
        public void Add(DocumentType documentType)
        {
            ArgumentNullException.ThrowIfNull(documentType);

            if (dbContext.DocumentTypes.Any(e => e.Abbreviation == documentType.Abbreviation
                                                 || e.Name == documentType.Name
                                                 || e.ExecutiveRepairDocNumber == documentType.ExecutiveRepairDocNumber))
            {
                throw new DataTransferException($"Document Type \"{documentType.Name}\" have already been add to the app (DB).");
            }

            var documentNormalize = documentTypeFactory.Create(documentType.Name, documentType.Abbreviation, documentType.ExecutiveRepairDocNumber, documentType.IsOnlyTypeDocInRepairLog);
            dbContext.DocumentTypes.Add(documentNormalize);
            dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = dbContext.DocumentTypes.FirstOrDefault(e => e.Id == id)
                        ?? throw new DataTransferException($"The ID for the document type  with \"{id}\" is already taken.");

            dbContext.DocumentTypes.Remove(item);
            dbContext.SaveChanges();
        }

        public DocumentType? GetDocumentType(Guid id)
            => dbContext.DocumentTypes.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public DocumentType? GetDocumentType(string abbreviation)
            => dbContext.DocumentTypes.AsNoTracking().FirstOrDefault(e => e.Abbreviation == abbreviation);
    }
}
