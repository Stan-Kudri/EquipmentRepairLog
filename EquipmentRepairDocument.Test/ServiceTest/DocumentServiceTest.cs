using EquipmentRepairDocument.Core.Data.DocumentModel;
using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.Data.ValidationData;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions.AppException;
using EquipmentRepairDocument.Core.FactoryData;
using EquipmentRepairDocument.Core.Service;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class DocumentServiceTest
    {
        [Fact(DisplayName = "Add document from ERD should add document without creat ERD")]
        public async Task AddDocumentFromERDAsync_Should_Add_Document_Without_Creat_ERD()
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);
            var documentService = new DocumentService(dbContext, documentFactory);

            var kksEquipmentRequestFirst = new KKSEquipmentRequest("20KAA22AA345 -- 20KAA21AA345 20KAA22AA345", "Клапан запорный", "НГ-2265");

            var division = new DivisionFactory().Create("Реакторный цех", "РЦ", 21);
            var documentType = new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 15, true);
            var repairFacility = new RepairFacility() { Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var perfomer = new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" };
            var erd = new ExecuteRepairDocument();

            await dbContext.Perfomers.AddRangeAsync(perfomer);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(erd);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.ExecuteRepairDocuments.AddAsync(erd);
            await dbContext.SaveChangesAsync();

            var requestFirst = new DocumentCreateRequest
            {
                RegistrationDate = DateTime.UtcNow,
                RepairDate = DateTime.UtcNow,
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = new List<KKSEquipmentRequest>() { kksEquipmentRequestFirst },
                PerfomersId = new List<Guid>() { perfomer.Id },
                ExecuteRepairDocumentsId = new List<Guid>() { erd.Id, },
            };

            var document = await documentFactory.CreateDocumentAsync(requestFirst, CancellationToken.None);
            await dbContext.Documents.AddAsync(document, CancellationToken.None);
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var kksEquipmentRequestSecond = new KKSEquipmentRequest("20KAA22AA366 20KAA21AA335", "Клапан запорный", "НГ-2265");
            var registrationNumber = document.RegistrationNumber;

            var requestSecond = new DocumentCreateRequest
            {
                RegistrationDate = DateTime.UtcNow,
                RepairDate = DateTime.UtcNow,
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = new List<KKSEquipmentRequest>() { kksEquipmentRequestSecond },
                PerfomersId = new List<Guid>() { perfomer.Id },
            };

            dbContext.ChangeTracker.Clear();

            // Act
            await documentService.AddDocumentFromERDAsync(requestSecond, registrationNumber, CancellationToken.None);

            // Assert - Two documents for one executive repair documentation 
            (await dbContext.Documents.CountAsync()).Should().Be(2);
        }

        [Fact(DisplayName = "Remove document should remove Exist Repair Document")]
        public async Task RemoveDocument_Existing_Should_Not_Throw_And_Remove()
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);
            var documentService = new DocumentService(dbContext, documentFactory);

            var kksEquipmentRequest = new KKSEquipmentRequest("20KAA22AA345 -- 20KAA21AA345 20KAA22AA345", "Клапан запорный", "НГ-2265");

            var division = new DivisionFactory().Create("Реакторный цех", "РЦ", 21);
            var documentType = new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 15, false);
            var repairFacility = new RepairFacility() { Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var perfomer = new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" };
            var erd = new ExecuteRepairDocument();

            await dbContext.Perfomers.AddRangeAsync(perfomer);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(erd);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.ExecuteRepairDocuments.AddAsync(erd);
            await dbContext.SaveChangesAsync();

            var executeRepairDocumnets = new List<ExecuteRepairDocument> { erd };

            var request = new DocumentCreateRequest
            {
                RegistrationDate = DateTime.UtcNow,
                RepairDate = DateTime.UtcNow,
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = new List<KKSEquipmentRequest>() { kksEquipmentRequest },
                ExecuteRepairDocumentsId = executeRepairDocumnets.Select(e => e.Id).ToList(),
                PerfomersId = new List<Guid>() { perfomer.Id },
            };

            var document = await documentFactory.CreateDocumentAsync(request, CancellationToken.None);
            await dbContext.Documents.AddAsync(document, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            dbContext.ChangeTracker.Clear();

            // Act
            Func<Task> result = () => documentService.RemoveDocument(document.Id);

            // Assert
            await result.Should().NotThrowAsync();
            (await dbContext.Documents.CountAsync()).Should().Be(0);
        }

        [Fact(DisplayName = "Remove document should throw when not found")]
        public async Task RemoveDocument_Not_Found_Should_Throw()
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);
            var documentService = new DocumentService(dbContext, documentFactory);
            var missingId = Guid.NewGuid();

            // Act
            Func<Task> act = () => documentService.RemoveDocument(missingId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Not found document to delete by '{missingId}'.");
        }

        [Fact(DisplayName = "Remove ERD should throw when RegistrationNumber not found")]
        public async Task Remove_ERD_Async_Not_Found_Should_Throw()
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);
            var documentService = new DocumentService(dbContext, documentFactory);

            // Act
            var result = () => documentService.RemoveERDAsync("Unknown");

            // Assert
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("Document with registration number \"Unknown\" not found.");
        }

        [Fact(DisplayName = "Remove ERD should delete ERD And link documents")]
        public async Task RemoveERDAsync_Existing_Should_Delete_ERD_And_Docs()
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);
            var documentService = new DocumentService(dbContext, documentFactory);

            var kksEquipmentRequestFirst = new KKSEquipmentRequest("20KAA22AA345 -- 20KAA21AA345 20KAA22AA345", "Клапан запорный", "НГ-2265");
            var kksEquipmentRequestSecond = new KKSEquipmentRequest("20KAA22AA366 20KAA21AA335", "Клапан запорный", "НГ-2265");

            var division = new DivisionFactory().Create("Реакторный цех", "РЦ", 21);
            var documentType = new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 15, true);
            var repairFacility = new RepairFacility() { Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var perfomer = new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" };
            var erd = new ExecuteRepairDocument();

            await dbContext.Perfomers.AddRangeAsync(perfomer);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(erd);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.ExecuteRepairDocuments.AddAsync(erd);
            await dbContext.SaveChangesAsync();

            var executeRepairDocumnets = new List<ExecuteRepairDocument> { erd };

            var requestFirst = new DocumentCreateRequest
            {
                RegistrationDate = DateTime.UtcNow,
                RepairDate = DateTime.UtcNow,
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = new List<KKSEquipmentRequest>() { kksEquipmentRequestFirst },
                PerfomersId = new List<Guid>() { perfomer.Id },
                ExecuteRepairDocumentsId = executeRepairDocumnets.Select(e => e.Id).ToList(),
            };

            var requestSecond = new DocumentCreateRequest
            {
                RegistrationDate = DateTime.UtcNow,
                RepairDate = DateTime.UtcNow,
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = new List<KKSEquipmentRequest>() { kksEquipmentRequestSecond },
                PerfomersId = new List<Guid>() { perfomer.Id },
                ExecuteRepairDocumentsId = executeRepairDocumnets.Select(e => e.Id).ToList(),
            };

            var documentFirst = await documentFactory.CreateDocumentAsync(requestFirst, CancellationToken.None);
            await dbContext.Documents.AddAsync(documentFirst, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            var documentSecond = await documentFactory.CreateDocumentAsync(requestSecond, CancellationToken.None);
            await dbContext.Documents.AddAsync(documentSecond, CancellationToken.None);
            await dbContext.SaveChangesAsync();

            var regNumberFirstDocument = documentFirst.RegistrationNumber;
            var regNumberSecondDocument = documentSecond.RegistrationNumber;
            var erdId = erd.Id;

            dbContext.ChangeTracker.Clear();

            // Act
            await documentService.RemoveERDAsync(regNumberSecondDocument, CancellationToken.None);

            // Assert
            (await dbContext.ExecuteRepairDocuments.FindAsync(erdId)).Should().BeNull("ERD must be deleted");
            (await dbContext.Documents.Where(d => d.RegistrationNumber == regNumberFirstDocument).ToListAsync()).Should().BeEmpty("All linked documents must be deleted");
        }

        [Fact(DisplayName = "Should throw NotFoundException when document does not exist")]
        // Test NotFoundException thrown
        public async Task RemoveDocument_Should_Throw_When_Document_Does_Not_Exist()
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);
            var documentService = new DocumentService(dbContext, documentFactory);

            // Act & Assert
            var result = async () => await documentService.RemoveDocument(Guid.NewGuid());
            await result.Should().ThrowAsync<NotFoundException>().WithMessage("*Not found*");
        }
    }
}
