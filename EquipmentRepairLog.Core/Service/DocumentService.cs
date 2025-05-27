using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentService(AppDbContext dbContext)
    {
        public async Task AddAllDocumentsAsync(List<Document> documents, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documents);

            if (documents.Count == 0)
            {
                throw new BusinessLogicException("The document does not contain any item.");
            }

            if (!IsFreePropertyDocuments(documents))
            {
                var invalidDocuments = documents.Where(document => dbContext.Documents.FirstOrDefault(e
                                                => (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                || e.RegistrationNumber == document.RegistrationNumber) == null).Select(document => new { document.RegistrationNumber }).ToList();
                throw new BusinessLogicException($"The document with registration number \"{string.Join(';', invalidDocuments.Select(e => e.RegistrationNumber))}\" is already taken.");
            }

            await dbContext.RunTransactionAsync(async _ =>
            {
                var executeRepairDocument = new ExecuteRepairDocument();
                await dbContext.ExecuteRepairDocuments.AddAsync(executeRepairDocument, cancellationToken);

                documents.ForEach(e => e?.ExecuteRepairDocuments?.Add(executeRepairDocument));
                await dbContext.Documents.AddRangeAsync(documents, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocument(Document document, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(document);
            BusinessLogicException.ThrowIfNull(document.ExecuteRepairDocuments);

            if (!IsFreePropertyDocument(document))
            {
                throw new BusinessLogicException($"The document with registration number \"{document.RegistrationNumber}\" and order number \"{document.OrdinalNumber}\" is already taken.");
            }

            await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание нового комплекта документа(ов)
                var executeRepairDocument = new ExecuteRepairDocument();
                await dbContext.ExecuteRepairDocuments.AddAsync(executeRepairDocument, cancellationToken);

                // Добавление документа и связь его с комплектом документа(ов)
                document.ExecuteRepairDocuments.Add(executeRepairDocument);
                await dbContext.Documents.AddAsync(document, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocumentFromERDAsync(Document document, string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(document);
            BusinessLogicException.ThrowIfNull(document.ExecuteRepairDocuments);

            if (!IsFreePropertyDocument(document))
            {
                throw new BusinessLogicException($"The document with registration number \"{document.RegistrationNumber}\" and order number \"{document.OrdinalNumber}\" is already taken.");
            }

            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments).FirstOrDefault(e => e.RegistrationNumber == registrationNumberDoc)
                                                                    ?? throw new BusinessLogicException($"Registration number {registrationNumberDoc} not found.");

            var executeRepairDoc = docByRegistrationNumber?.ExecuteRepairDocuments?.FirstOrDefault()
                                    ?? throw new BusinessLogicException("Empty Execute Repair Document.");

            await dbContext.RunTransactionAsync(async _ =>
            {
                document.ExecuteRepairDocuments.Add(executeRepairDoc);
                await dbContext.Documents.AddAsync(document, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task RemoveDocument(Guid id)
        {
            var count = await dbContext.Documents.Where(e => e.Id == id).ExecuteDeleteAsync();
            if (count == 0)
            {
                throw new NotFoundException($"Not found document to delete by '{id}'.");
            }
        }

        public async Task RemoveERDAsync(string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments)
                                                             .Where(e => e.RegistrationNumber == registrationNumberDoc)
                                                             .Select(e => new { e.ExecuteRepairDocuments, })
                                                             .FirstOrDefault();

            if (docByRegistrationNumber == null)
            {
                throw new NotFoundException($"Document with registration number \"{registrationNumberDoc}\" not found.");
            }

            if (docByRegistrationNumber.ExecuteRepairDocuments != null && docByRegistrationNumber.ExecuteRepairDocuments.Count == 0)
            {
                throw new BusinessLogicException($"Document with registration number {registrationNumberDoc} does not belong to any set of executive repair documentation.");
            }

            var executeRepairDocuments = docByRegistrationNumber.ExecuteRepairDocuments;
            BusinessLogicException.ThrowIfNull(executeRepairDocuments);

            await dbContext.RunTransactionAsync(async _ =>
            {
                var executeRepairDocumentsId = executeRepairDocuments.ConvertAll(e => e.Id);
                var documents = dbContext.ExecuteRepairDocuments.Include(erd => erd.Documents).Where(e => executeRepairDocumentsId.Contains(e.Id)).SelectMany(e => e.Documents).Distinct();

                dbContext.Documents.RemoveRange(documents);
                dbContext.ExecuteRepairDocuments.RemoveRange(executeRepairDocuments);
                await dbContext.SaveChangesAsync(cancellationToken);

                return DBNull.Value;
            },
            cancellationToken);
        }

        private bool IsFreePropertyDocuments(List<Document> documents)
        {
            var registrationNumber = documents.Select(e => e.RegistrationNumber);
            return !dbContext.Documents.Include(e => e.DocumentType).Any(document => registrationNumber.Contains(document.RegistrationNumber));
        }

        private bool IsFreePropertyDocument(Document document)
            => !dbContext.Documents.Include(e => e.DocumentType).Any(e => e.RegistrationNumber == document.RegistrationNumber);
    }
}
