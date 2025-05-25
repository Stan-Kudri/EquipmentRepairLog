using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using EquipmentRepairLog.Core.FactoryData;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentTypeService(AppDbContext dbContext, DocumentTypeFactory documentTypeFactory)
    {
        public async Task Add(DocumentType documentType)
        {
            BusinessLogicException.ThrowIfNull(documentType);

            var existingDocumentType = dbContext.DocumentTypes.FirstOrDefault(e => e.Abbreviation == documentType.Abbreviation
                                                                           || e.Name == documentType.Name
                                                                           || e.ExecutiveRepairDocNumber == documentType.ExecutiveRepairDocNumber);

            if (existingDocumentType == null)
            {
                var documentNormalize = documentTypeFactory.Create(documentType.Name, documentType.Abbreviation, documentType.ExecutiveRepairDocNumber, documentType.IsOnlyTypeDocInRepairLog);
                await dbContext.DocumentTypes.AddAsync(documentNormalize);
                await dbContext.SaveChangesAsync();
                return;
            }

            if (existingDocumentType.Abbreviation == documentType.Abbreviation)
            {
                BusinessLogicException.EnsureUniqueProperty<Division>(existingDocumentType.Abbreviation);
            }
            if (existingDocumentType.Name == documentType.Name)
            {
                BusinessLogicException.EnsureUniqueProperty<Division>(existingDocumentType.Name);
            }
            if (existingDocumentType.ExecutiveRepairDocNumber == documentType.ExecutiveRepairDocNumber)
            {
                BusinessLogicException.EnsureUniqueProperty<Division>(existingDocumentType.ExecutiveRepairDocNumber);
            }
        }

        public async Task Remove(Guid id)
        {
            var count = await dbContext.DocumentTypes.Where(e => e.Id == id).ExecuteDeleteAsync();
            if (count == 0)
            {
                throw new NotFoundException($"The ID for the document type  with \"{id}\" is already taken.");
            }
        }
    }
}
