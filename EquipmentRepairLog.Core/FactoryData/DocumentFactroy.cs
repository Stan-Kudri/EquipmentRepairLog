using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.FactoryData
{
    public class DocumentFactroy(AppDbContext dbContext)
    {
        public async Task<List<Document>> CreateListDocumentAsync(List<DocumentCreateRequest> documentCreateRequests, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentCreateRequests);

            if (documentCreateRequests.Count == 0)
            {
                throw new BusinessLogicException("The document does not contain any item.");
            }

            var result = new List<Document>(documentCreateRequests.Count);

            // Создание/установка порядкового и регистрационного номера
            foreach (var item in documentCreateRequests)
            {
                var document = await CreateDocumentAsync(item, cancellationToken);
                result.Add(document);
            }

            return result;
        }

        public async Task<Document> CreateDocumentAsync(DocumentCreateRequest documentCreateRequest, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentCreateRequest);
            BusinessLogicException.ThrowIfNull(documentCreateRequest.ExecuteRepairDocuments);

            // Создание/установка порядкового и регистрационного номера
            var (OrdinalNumber, RegistrationNumber) = await GetNumberDocument(documentCreateRequest, cancellationToken);

            return new Document()
            {
                Division = documentCreateRequest.Division,
                DocumentType = documentCreateRequest.DocumentType,
                RepairFacility = documentCreateRequest.RepairFacility,
                RepairDate = documentCreateRequest.RepairDate,
                KKSEquipment = documentCreateRequest.KKSEquipment,
                Perfomers = documentCreateRequest.Perfomers,
                DivisionId = documentCreateRequest.DivisionId,
                DocumentTypeId = documentCreateRequest.DocumentTypeId,
                RepairFacilityId = documentCreateRequest.RepairFacilityId,
                RegistrationDate = documentCreateRequest.RegistrationDate,
                ExecuteRepairDocuments = documentCreateRequest.ExecuteRepairDocuments,
                OrdinalNumber = OrdinalNumber,
                RegistrationNumber = RegistrationNumber,
            };
        }

        public async Task<Document> CreateDocumentFromERDAsync(DocumentCreateRequest documentCreateRequest, string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentCreateRequest);
            BusinessLogicException.ThrowIfNull(documentCreateRequest.ExecuteRepairDocuments);

            var executeRepairDoc = await dbContext.Documents.Include(erd => erd.ExecuteRepairDocuments)
                                                             .Where(e => e.RegistrationNumber == registrationNumberDoc)
                                                             .SelectMany(e => e.ExecuteRepairDocuments)
                                                             .FirstOrDefaultAsync(cancellationToken)
                                                             ?? throw NotFoundException.Create<ExecuteRepairDocument, string>(nameof(registrationNumberDoc), registrationNumberDoc);

            documentCreateRequest.ExecuteRepairDocuments.Add(executeRepairDoc);
            return await CreateDocumentAsync(documentCreateRequest, cancellationToken);
        }

        private async Task<(int OrdinalNumber, string RegistrationNumber)> GetNumberDocument(DocumentCreateRequest documentCreateRequest, CancellationToken cancellationToken)
        {
            var divisionNumber = await dbContext.Divisions.Where(e => e.Id == documentCreateRequest.DivisionId).Select(e => (byte?)e.Number).FirstOrDefaultAsync(cancellationToken)
                                    ?? throw NotFoundException.Create<Division, Guid>(nameof(documentCreateRequest.DivisionId), documentCreateRequest.DivisionId);

            var repairFacilityNumber = await dbContext.RepairFacilities.Where(e => e.Id == documentCreateRequest.RepairFacilityId).Select(e => (byte?)e.Number).FirstOrDefaultAsync(cancellationToken)
                                        ?? throw NotFoundException.Create<RepairFacility, Guid>(nameof(documentCreateRequest.RepairFacilityId), documentCreateRequest.RepairFacilityId);

            var typeDocAbbreviation = await dbContext.DocumentTypes.Where(e => e.Id == documentCreateRequest.DocumentTypeId).Select(e => e.Abbreviation).FirstOrDefaultAsync(cancellationToken)
                                        ?? throw NotFoundException.Create<DocumentType, Guid>(nameof(documentCreateRequest.DocumentTypeId), documentCreateRequest.DocumentTypeId);

            var ordinalNumber = await GetOrdinalNumberAsync(documentCreateRequest, cancellationToken);

            var year = documentCreateRequest.RegistrationDate.Year;

            var registrationNumber = $"{ordinalNumber}/{divisionNumber}{typeDocAbbreviation}({repairFacilityNumber})-{year}";
            return (ordinalNumber, registrationNumber);
        }

        private async Task<int> GetOrdinalNumberAsync(DocumentCreateRequest documentCreateRequest, CancellationToken cancellationToken)
        {
            var ordinalNumber = await dbContext.Documents.Where(e => e.DivisionId == documentCreateRequest.DivisionId
                                                                     && e.RegistrationDate.Year == documentCreateRequest.RegistrationDate.Year)
                                                         .OrderByDescending(e => e.OrdinalNumber)
                                                         .Select(e => e.OrdinalNumber)
                                                         .FirstOrDefaultAsync(cancellationToken);

            return ordinalNumber == 0 ? 1 : ordinalNumber + 1;
        }
    }
}
