using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentService(AppDbContext dbContext)
    {
        public async Task AddAllDocumentsAsync(List<DocumentModelCreater> documentsCreator, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentsCreator);

            if (documentsCreator.Count == 0)
            {
                throw new BusinessLogicException("The document does not contain any item.");
            }

            if (!IsFreePropertyDocuments(documentsCreator))
            {
                var invalidDocuments = documentsCreator.Where(document => dbContext.Documents.FirstOrDefault(e
                                                => (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                || e.RegistrationNumber == document.RegistrationNumber) == null).Select(document => new { document.RegistrationNumber }).ToList();
                throw new BusinessLogicException($"The document with registration number \"{string.Join(';', invalidDocuments.Select(e => e.RegistrationNumber))}\" is already taken.");
            }

            await dbContext.RunTransactionAsync(async _ =>
            {
                var executeRepairDocument = new ExecuteRepairDocument();
                await dbContext.ExecuteRepairDocuments.AddAsync(executeRepairDocument, cancellationToken);

                foreach (var item in documentsCreator)
                {
                    var numberDoc = await GetRegistrationNumber(item, cancellationToken);
                    item.RegistrationNumber = numberDoc.RegistrationNumber;
                    item.OrdinalNumber = numberDoc.OrdinalNumber;
                }

                documentsCreator.ForEach(e => e?.ExecuteRepairDocuments?.Add(executeRepairDocument));
                var documents = documentsCreator.Select(d => d.GetDocument()).ToList();

                await dbContext.Documents.AddRangeAsync(documents, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocument(DocumentModelCreater documentsCreator, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentsCreator);
            BusinessLogicException.ThrowIfNull(documentsCreator.ExecuteRepairDocuments);

            if (!IsFreePropertyDocument(documentsCreator))
            {
                throw new BusinessLogicException($"The document with registration number \"{documentsCreator.RegistrationNumber}\" and order number \"{documentsCreator.OrdinalNumber}\" is already taken.");
            }

            await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание нового комплекта документа(ов)
                var executeRepairDocument = new ExecuteRepairDocument();
                await dbContext.ExecuteRepairDocuments.AddAsync(executeRepairDocument, cancellationToken);

                // Добавление документа и связь его с комплектом документа(ов)
                documentsCreator.ExecuteRepairDocuments.Add(executeRepairDocument);
                var numberDoc = await GetRegistrationNumber(documentsCreator, cancellationToken);
                documentsCreator.RegistrationNumber = numberDoc.RegistrationNumber;
                documentsCreator.OrdinalNumber = numberDoc.OrdinalNumber;
                var document = documentsCreator.GetDocument();

                await dbContext.Documents.AddAsync(document, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocumentFromERDAsync(DocumentModelCreater documentsCreator, string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentsCreator);
            BusinessLogicException.ThrowIfNull(documentsCreator.ExecuteRepairDocuments);

            if (!IsFreePropertyDocument(documentsCreator))
            {
                throw new BusinessLogicException($"The document with registration number \"{documentsCreator.RegistrationNumber}\" and order number \"{documentsCreator.OrdinalNumber}\" is already taken.");
            }

            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments).FirstOrDefault(e => e.RegistrationNumber == registrationNumberDoc)
                                                                    ?? throw new BusinessLogicException($"Registration number {registrationNumberDoc} not found.");

            var executeRepairDoc = docByRegistrationNumber?.ExecuteRepairDocuments?.FirstOrDefault()
                                    ?? throw new BusinessLogicException("Empty Execute Repair Document.");

            await dbContext.RunTransactionAsync(async _ =>
            {
                var numberDoc = await GetRegistrationNumber(documentsCreator, cancellationToken);
                documentsCreator.RegistrationNumber = numberDoc.RegistrationNumber;
                documentsCreator.OrdinalNumber = numberDoc.OrdinalNumber;
                documentsCreator.ExecuteRepairDocuments.Add(executeRepairDoc);
                var document = documentsCreator.GetDocument();

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

        private bool IsFreePropertyDocuments(List<DocumentModelCreater> documents)
        {
            var registrationNumber = documents.Select(e => e.RegistrationNumber);
            return !dbContext.Documents.Include(e => e.DocumentType).Any(document => registrationNumber.Contains(document.RegistrationNumber));
        }

        private bool IsFreePropertyDocument(DocumentModelCreater document)
            => !dbContext.Documents.Include(e => e.DocumentType).Any(e => e.RegistrationNumber == document.RegistrationNumber);

        private async Task<(int OrdinalNumber, string RegistrationNumber)> GetRegistrationNumber(DocumentModelCreater document, CancellationToken cancellationToken)
        {
            var ordinalNumber = document.OrdinalNumber = await GetOrdinalNumber(document);

            var divisionNumber = (await dbContext.Divisions.FirstOrDefaultAsync(e => e.Id == document.DivisionId, cancellationToken))?.Number
                                    ?? throw new BusinessLogicException("Error. The division ID not found.");

            var repairFacilityNumber = (await dbContext.RepairFacilities.FirstOrDefaultAsync(e => e.Id == document.RepairFacilityId, cancellationToken))?.Number
                                        ?? throw new BusinessLogicException("Error. The repair facility ID not found.");

            var typeDocAbbreviation = (await dbContext.DocumentTypes.FirstOrDefaultAsync(e => e.Id == document.DocumentTypeId, cancellationToken))?.Abbreviation
                                        ?? throw new BusinessLogicException("Error. The repair facility ID not found.");

            var year = document.RegistrationDate.Year;

            var registrationNumber = $"{ordinalNumber}/{divisionNumber}{typeDocAbbreviation}({repairFacilityNumber})-{year}";
            return (ordinalNumber, registrationNumber);
        }

        private async Task<int> GetOrdinalNumber(DocumentModelCreater document)
        {
            var ordinalNumber = await dbContext.Documents.Where(e => e.DivisionId == document.DivisionId
                                                                     && e.RegistrationDate.Year == document.RegistrationDate.Year)
                                                         .OrderByDescending(e => e.OrdinalNumber)
                                                         .Select(e => e.OrdinalNumber)
                                                         .FirstOrDefaultAsync();

            return ordinalNumber == 0 ? 1 : ordinalNumber + 1;
        }
    }
}
