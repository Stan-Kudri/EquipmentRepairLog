using EquipmentRepairDocument.Core.Data.DocumentModel;
using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Exceptions.AppException;
using EquipmentRepairDocument.Core.Service;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Core.FactoryData
{
    public class DocumentFactory(AppDbContext dbContext, EquipmentService equipmentService)
    {
        public async Task<Document> CreateDocumentAsync(DocumentCreateRequest documentCreateRequest, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(documentCreateRequest);
            BusinessLogicException.ThrowIfNull(documentCreateRequest.ExecuteRepairDocuments);
            BusinessLogicException.ThrowIfNullOrEmptyCollection(documentCreateRequest.KKSEquipment);
            BusinessLogicException.ThrowIfNullOrEmptyCollection(documentCreateRequest.Perfomers);

            var kksIdEquipments = await equipmentService.AddRangeEquipmentAsync(documentCreateRequest.KKSEquipment);
            var kksEquipment = dbContext.KKSEquipments.Where(kks => kksIdEquipments.Contains(kks.Id));
            BusinessLogicException.ThrowIfNullOrEmptyCollection(kksEquipment);

            // Создание/установка порядкового и регистрационного номера
            var (OrdinalNumber, RegistrationNumber) = await GetNumberDocument(documentCreateRequest, cancellationToken);

            return new Document()
            {
                Division = documentCreateRequest.Division,
                DocumentType = documentCreateRequest.DocumentType,
                RepairFacility = documentCreateRequest.RepairFacility,
                RepairDate = documentCreateRequest.RepairDate,
                KKSEquipment = kksEquipment.ToList(),
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
            BusinessLogicException.ThrowIfNullOrEmptyCollection(documentCreateRequest.KKSEquipment);
            BusinessLogicException.ThrowIfNullOrEmptyCollection(documentCreateRequest.Perfomers);

            var executeRepairDoc = await dbContext.Documents.Include(erd => erd.ExecuteRepairDocuments)
                                                             .AsNoTracking()
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
