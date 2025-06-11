using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Exceptions.AppException;
using EquipmentRepairDocument.Core.FactoryData;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Core.Service
{
    public class DocumentTypeService(AppDbContext dbContext, DocumentTypeFactory documentTypeFactory)
    {
        public async Task AddRangeAsync(List<DocumentType> documentTypes, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentTypes);
            foreach (var document in documentTypes)
            {
                await AddAsync(document, cancellationToken);
            }
        }

        public async Task AddAsync(DocumentType documentType, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentType);

            var existingDocumentType = dbContext.DocumentTypes.FirstOrDefault(e => e.Abbreviation == documentType.Abbreviation
                                                                           || e.Name == documentType.Name
                                                                           || e.ExecutiveRepairDocNumber == documentType.ExecutiveRepairDocNumber);

            if (existingDocumentType == null)
            {
                var documentNormalize = documentTypeFactory.Create(documentType.Name, documentType.Abbreviation, documentType.ExecutiveRepairDocNumber, documentType.IsOnlyTypeDocInERD);
                await dbContext.DocumentTypes.AddAsync(documentNormalize, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
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

        public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var count = await dbContext.DocumentTypes.Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);
            if (count == 0)
            {
                throw new NotFoundException($"The ID for the document type  with \"{id}\" is already taken.");
            }
        }

        public async Task RemoveAsync(byte number, CancellationToken cancellationToken = default)
        {
            var count = await dbContext.DocumentTypes.Where(e => e.ExecutiveRepairDocNumber == number).ExecuteDeleteAsync(cancellationToken);
            if (count == 0)
            {
                throw new NotFoundException($"The document type number \"{number}\" not found.");
            }
        }
    }
}
