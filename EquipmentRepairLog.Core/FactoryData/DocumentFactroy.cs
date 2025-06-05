using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.FactoryData
{
    public class DocumentFactroy(AppDbContext dbContext)
    {
        public async Task<List<Document>> GetListDocumentAsync(List<DocumentModelCreator> documentsCreator, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentsCreator);

            if (documentsCreator.Count == 0)
            {
                throw new BusinessLogicException("The document does not contain any item.");
            }

            // Создание/установка порядкового и регистрационного номера
            foreach (var item in documentsCreator)
            {
                var (OrdinalNumber, RegistrationNumber) = await GetNumberDocument(item, cancellationToken);
                item.SetNumberDocument(OrdinalNumber, RegistrationNumber);
            }

            return documentsCreator.Select(e => e.GetDocument()).ToList();
        }

        public async Task<Document> GetDocumentAsync(DocumentModelCreator documentsCreator, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentsCreator);
            BusinessLogicException.ThrowIfNull(documentsCreator.ExecuteRepairDocuments);

            // Создание/установка порядкового и регистрационного номера
            var (OrdinalNumber, RegistrationNumber) = await GetNumberDocument(documentsCreator, cancellationToken);
            documentsCreator.SetNumberDocument(OrdinalNumber, RegistrationNumber);

            return documentsCreator.GetDocument();
        }

        public async Task<IQueryable<Document>> GetRemoveDocsByRegistrationNumberAsync(string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            var executeRepairDocuments = await dbContext.Documents.Include(e => e.ExecuteRepairDocuments)
                                                                  .Where(e => e.RegistrationNumber == registrationNumberDoc)
                                                                  .Select(e => e.ExecuteRepairDocuments)
                                                                  .FirstOrDefaultAsync(cancellationToken)
                                                                  ?? throw new NotFoundException($"Document with registration number \"{registrationNumberDoc}\" not found.");

            var executeRepairDocumentsId = executeRepairDocuments.ConvertAll(e => e.Id);
            var documents = dbContext.ExecuteRepairDocuments.Include(erd => erd.Documents).Where(e => executeRepairDocumentsId.Contains(e.Id)).SelectMany(e => e.Documents).Distinct();
            return documents;
        }

        public async Task<Document> GetDocumentFromERDAsync(DocumentModelCreator documentsCreator, string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentsCreator);
            BusinessLogicException.ThrowIfNull(documentsCreator.ExecuteRepairDocuments);

            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments).FirstOrDefault(e => e.RegistrationNumber == registrationNumberDoc)
                                                                    ?? throw new BusinessLogicException($"Registration number {registrationNumberDoc} not found.");

            var executeRepairDoc = docByRegistrationNumber?.ExecuteRepairDocuments?.FirstOrDefault()
                                    ?? throw new BusinessLogicException("Empty Execute Repair Document.");

            documentsCreator.ExecuteRepairDocuments.Add(executeRepairDoc);

            // Создание/установка порядкового и регистрационного номера
            var (OrdinalNumber, RegistrationNumber) = await GetNumberDocument(documentsCreator, cancellationToken);
            documentsCreator.SetNumberDocument(OrdinalNumber, RegistrationNumber);

            return documentsCreator.GetDocument();
        }

        private async Task<(int OrdinalNumber, string RegistrationNumber)> GetNumberDocument(DocumentModelCreator document, CancellationToken cancellationToken)
        {
            var divisionNumber = await dbContext.Divisions.Where(e => e.Id == document.DivisionId).Select(e => (byte?)e.Number).FirstOrDefaultAsync(cancellationToken)
                                    ?? throw new NotFoundException("Error. The division ID not found.");

            var repairFacilityNumber = await dbContext.RepairFacilities.Where(e => e.Id == document.RepairFacilityId).Select(e => (byte?)e.Number).FirstOrDefaultAsync(cancellationToken)
                                        ?? throw new NotFoundException("Error. The repair facility ID not found.");

            var typeDocAbbreviation = await dbContext.DocumentTypes.Where(e => e.Id == document.DocumentTypeId).Select(e => e.Abbreviation).FirstOrDefaultAsync(cancellationToken)
                                        ?? throw new NotFoundException("Error. The document type ID not found.");

            var ordinalNumber = await GetOrdinalNumberAsync(document, cancellationToken);

            var year = document.RegistrationDate.Year;

            var registrationNumber = $"{ordinalNumber}/{divisionNumber}{typeDocAbbreviation}({repairFacilityNumber})-{year}";
            return (ordinalNumber, registrationNumber);
        }

        private async Task<int> GetOrdinalNumberAsync(DocumentModelCreator document, CancellationToken cancellationToken)
        {
            var ordinalNumber = await dbContext.Documents.Where(e => e.DivisionId == document.DivisionId
                                                                     && e.RegistrationDate.Year == document.RegistrationDate.Year)
                                                         .OrderByDescending(e => e.OrdinalNumber)
                                                         .Select(e => e.OrdinalNumber)
                                                         .FirstOrDefaultAsync(cancellationToken);

            return ordinalNumber == 0 ? 1 : ordinalNumber + 1;
        }
    }
}
