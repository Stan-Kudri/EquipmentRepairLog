using EquipmentRepairDocument.Core.Data.DocumentModel;
using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.Data.ValidationData;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Exceptions.AppException;
using EquipmentRepairDocument.Core.FactoryData;
using EquipmentRepairDocument.Core.Service;
using FluentAssertions;

namespace EquipmentRepairDocument.Test.FactoryTests
{
    public class DocumentFactoryTests
    {
        [Fact(DisplayName = "CreateDocumentFromERDAsync valid RegistrationNumber should add ERD and CreateDocument")]
        public async Task CreateDocumentFromERDAsync_ValidRegistrationNumber_ShouldAddERDAndCreateDocument()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, equipmentService);

            var division = new Division { Id = Guid.NewGuid(), Number = 21, Name = "Реакторный цех", Abbreviation = "РЦ" };
            var repairFacility = new RepairFacility() { Id = Guid.NewGuid(), Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var documentType = new DocumentType { Id = Guid.NewGuid(), Name = "Ведомость выполненных работ", Abbreviation = "ВВР", ExecutiveRepairDocNumber = 15, MultipleUseInERD = true };
            var perfomers = new List<Perfomer> { new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" } };
            var executeRepairDocs = new List<ExecuteRepairDocument> { new() { Id = Guid.NewGuid() } };

            await dbContext.Perfomers.AddRangeAsync(perfomers);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(executeRepairDocs);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.SaveChangesAsync(CancellationToken.None);
            dbContext.ChangeTracker.Clear();

            var kksRequests = new List<KKSEquipmentRequest> { new KKSEquipmentRequest("20KAA22AA345 -- ", "Клапан запорный", "НГ-2265") };

            var requestFirst = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = kksRequests,
                PerfomersId = perfomers.Select(e => e.Id).ToList(),
                ExecuteRepairDocumentsId = executeRepairDocs.Select(e => e.Id).ToList(),
            };

            var documentFirst = await documentFactory.CreateDocumentAsync(requestFirst, CancellationToken.None);
            await dbContext.Documents.AddAsync(documentFirst);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var requestSecond = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = new List<KKSEquipmentRequest>() { new KKSEquipmentRequest("20KAA21AA345 20KAA22AA345", "Клапан запорный", "НГ-2265") },
                PerfomersId = perfomers.Select(e => e.Id).ToList(),
            };

            // Act
            var result = await documentFactory.CreateDocumentFromERDAsync(requestSecond, documentFirst.RegistrationNumber!, CancellationToken.None);

            // Assert
            result.ExecuteRepairDocuments.Should().ContainInOrder(executeRepairDocs);
        }

        [Fact(DisplayName = "CreateDocumentAsync Division not found should throw NotFoundException")]
        public async Task CreateDocumentAsync_DivisionNotFound_ShouldThrowNotFoundException()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, equipmentService);

            var repairFacility = new RepairFacility() { Id = Guid.NewGuid(), Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var documentType = new DocumentType { Id = Guid.NewGuid(), Name = "Ведомость выполненных работ", Abbreviation = "ВВР", ExecutiveRepairDocNumber = 15, MultipleUseInERD = true };
            var perfomers = new List<Perfomer> { new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" } };
            var executeRepairDocs = new List<ExecuteRepairDocument> { new() { Id = Guid.NewGuid() } };

            await dbContext.Perfomers.AddRangeAsync(perfomers);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(executeRepairDocs);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.SaveChangesAsync();

            var kksRequests = new List<KKSEquipmentRequest> { new KKSEquipmentRequest("20KAA22AA345 -- ", "Клапан запорный", "НГ-2265") };

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DocumentTypeId = documentType.Id,
                DivisionId = Guid.NewGuid(),
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = kksRequests,
                PerfomersId = perfomers.Select(e => e.Id).ToList(),
                ExecuteRepairDocumentsId = executeRepairDocs.Select(e => e.Id).ToList(),
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => documentFactory.CreateDocumentAsync(request, CancellationToken.None));
        }

        [Fact(DisplayName = "CreateDocumentAsync OrdinalNumber incremented correctly")]
        public async Task CreateDocumentAsync_OrdinalNumberIncrementedCorrectly()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, equipmentService);

            var division = new Division { Id = Guid.NewGuid(), Number = 21, Name = "Реакторный цех", Abbreviation = "РЦ" };
            var repairFacility = new RepairFacility() { Id = Guid.NewGuid(), Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var documentType = new DocumentType { Id = Guid.NewGuid(), Name = "Ведомость выполненных работ", Abbreviation = "ВВР", ExecutiveRepairDocNumber = 15, MultipleUseInERD = true };
            var perfomers = new List<Perfomer> { new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" } };
            var executeRepairDocs = new List<ExecuteRepairDocument> { new() { Id = Guid.NewGuid() } };

            var kksRequests = new KKSEquipmentRequest("20KAA22AA345 -- ", "Клапан запорный", "НГ-2265");

            var idEquipments = await equipmentService.AddEquipmentAsync(kksRequests);
            var kksEquipments = dbContext.KKSEquipments.Where(e => idEquipments.Contains(e.Id)).ToList();

            await dbContext.Perfomers.AddRangeAsync(perfomers);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(executeRepairDocs);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.Documents.AddAsync(new Document
            {
                DivisionId = division.Id,
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                OrdinalNumber = 1,
                DocumentTypeId = documentType.Id,
                ExecuteRepairDocuments = executeRepairDocs,
                RepairFacilityId = repairFacility.Id,
                Perfomers = perfomers,
                RegistrationNumber = "NumberRegistration",
                KKSEquipment = kksEquipments,
            });
            await dbContext.SaveChangesAsync();

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = new List<KKSEquipmentRequest>() { new KKSEquipmentRequest("20KAA21AA345 20KAA22AA345", "Клапан запорный", "НГ-2265") },
                PerfomersId = perfomers.Select(e => e.Id).ToList(),
                ExecuteRepairDocumentsId = executeRepairDocs.Select(e => e.Id).ToList(),
            };

            // Act
            var result = await documentFactory.CreateDocumentAsync(request, CancellationToken.None);

            // Assert
            result.OrdinalNumber.Should().Be(2);
            result.RegistrationNumber.Should().Be($"2/{division.Number}{documentType.Abbreviation}({repairFacility.Number})-{request.RegistrationDate.Year}");
        }

        [Fact(DisplayName = "CreateDocumentFromERDAsync ERD not found should throw NotFoundException")]
        public async Task CreateDocumentFromERDAsync_ERDNotFound_ShouldThrowNotFoundException()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, equipmentService);

            var division = new Division { Id = Guid.NewGuid(), Number = 21, Name = "Реакторный цех", Abbreviation = "РЦ" };
            var repairFacility = new RepairFacility() { Id = Guid.NewGuid(), Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var documentType = new DocumentType { Id = Guid.NewGuid(), Name = "Ведомость выполненных работ", Abbreviation = "ВВР", ExecutiveRepairDocNumber = 15, MultipleUseInERD = true };
            var perfomers = new List<Perfomer> { new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" } };
            var executeRepairDocs = new List<ExecuteRepairDocument> { new() { Id = Guid.NewGuid() } };

            await dbContext.Perfomers.AddRangeAsync(perfomers);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(executeRepairDocs);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.SaveChangesAsync();

            var kksRequests = new List<KKSEquipmentRequest> { new KKSEquipmentRequest("20KAA22AA345 -- ", "Клапан запорный", "НГ-2265") };

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = kksRequests,
                PerfomersId = perfomers.Select(e => e.Id).ToList(),
                ExecuteRepairDocumentsId = executeRepairDocs.Select(e => e.Id).ToList()
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => documentFactory.CreateDocumentFromERDAsync(request, "NonExistent", CancellationToken.None));
        }

        [Fact(DisplayName = "CreateDocumentAsync with valid request should return document with correct fields")]
        public async Task CreateDocumentAsync_WithValidRequest_ShouldReturnDocumentWithCorrectFields()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, equipmentService);

            var division = new Division { Id = Guid.NewGuid(), Number = 21, Name = "Реакторный цех", Abbreviation = "РЦ" };
            var repairFacility = new RepairFacility() { Id = Guid.NewGuid(), Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var documentType = new DocumentType { Id = Guid.NewGuid(), Name = "Ведомость выполненных работ", Abbreviation = "ВВР", ExecutiveRepairDocNumber = 15, MultipleUseInERD = true };
            var perfomers = new List<Perfomer> { new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" } };
            var executeRepairDocs = new List<ExecuteRepairDocument> { new() { Id = Guid.NewGuid() } };

            await dbContext.Perfomers.AddRangeAsync(perfomers);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(executeRepairDocs);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.SaveChangesAsync();

            var kksRequests = new List<KKSEquipmentRequest> { new KKSEquipmentRequest("20KAA22AA345 -- ", "Клапан запорный", "НГ-2265") };

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = kksRequests,
                PerfomersId = perfomers.Select(e => e.Id).ToList(),
                ExecuteRepairDocumentsId = executeRepairDocs.Select(e => e.Id).ToList(),
            };

            // Act
            var result = await documentFactory.CreateDocumentAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.RegistrationDate.Should().Be(request.RegistrationDate);
            result.DivisionId.Should().Be(request.DivisionId);
            result.RepairFacilityId.Should().Be(request.RepairFacilityId);
            result.KKSEquipment.Should().NotBeEmpty();
            result.Perfomers.Should().BeEquivalentTo(perfomers);
            result.ExecuteRepairDocuments.Should().BeEquivalentTo(executeRepairDocs);
            result.OrdinalNumber.Should().Be(1);
            result.RegistrationNumber.Should().Be($"1/{division.Number}{documentType.Abbreviation}({repairFacility.Number})-{request.RegistrationDate.Year}");
        }

        [Fact(DisplayName = "CreateDocumentAsync no KKS found after adding should throw BusinessLogicException")]
        public async Task CreateDocumentAsync_NoKKSFoundAfterAdding_ShouldThrowBusinessLogicException()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);

            var division = new Division { Id = Guid.NewGuid(), Number = 21, Name = "Реакторный цех", Abbreviation = "РЦ" };
            var repairFacility = new RepairFacility() { Id = Guid.NewGuid(), Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1", Number = 1 };
            var documentType = new DocumentType { Id = Guid.NewGuid(), Name = "Ведомость выполненных работ", Abbreviation = "ВВР", ExecutiveRepairDocNumber = 15, MultipleUseInERD = true };
            var perfomers = new List<Perfomer> { new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" } };
            var executeRepairDocs = new List<ExecuteRepairDocument> { new() { Id = Guid.NewGuid() } };

            await dbContext.Perfomers.AddRangeAsync(perfomers);
            await dbContext.ExecuteRepairDocuments.AddRangeAsync(executeRepairDocs);
            await dbContext.Divisions.AddAsync(division);
            await dbContext.RepairFacilities.AddAsync(repairFacility);
            await dbContext.DocumentTypes.AddAsync(documentType);
            await dbContext.SaveChangesAsync();

            var kksRequests = new List<KKSEquipmentRequest> { new KKSEquipmentRequest("20KAA22asdasdaAA345", "Клапан запорный", "НГ-2265") };

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DocumentTypeId = documentType.Id,
                DivisionId = division.Id,
                RepairFacilityId = repairFacility.Id,
                KKSEquipment = kksRequests,
                PerfomersId = perfomers.Select(e => e.Id).ToList(),
                ExecuteRepairDocumentsId = executeRepairDocs.Select(e => e.Id).ToList(),
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessLogicException>(() => documentFactory.CreateDocumentAsync(request, CancellationToken.None));
        }

        [Fact(DisplayName = "CreateDocumentAsync request is null should throw BusinessLogicException")]
        public async Task CreateDocumentAsync_RequestIsNull_ShouldThrowBusinessLogicException()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessLogicException>(() => documentFactory.CreateDocumentAsync(null!, CancellationToken.None));
        }

        [Fact(DisplayName = "CreateDocumentAsync KKSEquipment is empty should throw BusinessLogicException")]
        public async Task CreateDocumentAsync_KKSEquipmentIsEmpty_ShouldThrowBusinessLogicException()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DivisionId = Guid.NewGuid(),
                DocumentTypeId = Guid.NewGuid(),
                RepairFacilityId = Guid.NewGuid(),
                KKSEquipment = new List<KKSEquipmentRequest>(),
                PerfomersId = new List<Guid> { Guid.NewGuid() },
                ExecuteRepairDocumentsId = new List<Guid> { new() },
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessLogicException>(() => documentFactory.CreateDocumentAsync(request, CancellationToken.None));
        }

        [Fact(DisplayName = "CreateDocumentAsync Performers is empty should throw BusinessLogicException")]
        public async Task CreateDocumentAsync_PerformersIsEmpty_ShouldThrowBusinessLogicException()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DivisionId = Guid.NewGuid(),
                DocumentTypeId = Guid.NewGuid(),
                RepairFacilityId = Guid.NewGuid(),
                KKSEquipment = new List<KKSEquipmentRequest>() { new KKSEquipmentRequest("20KAA22AA366 20KAA21AA335", "Клапан запорный", "НГ-2265") },
                PerfomersId = new List<Guid>(),
                ExecuteRepairDocumentsId = new List<Guid> { Guid.NewGuid() },
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessLogicException>(() => documentFactory.CreateDocumentAsync(request, CancellationToken.None));
        }

        [Fact(DisplayName = "CreateDocumentAsync ExecuteRepairDocuments is null should throw BusinessLogicException")]
        public async Task CreateDocumentAsync_ExecuteRepairDocumentsIsNull_ShouldThrowBusinessLogicException()
        {
            // Arrange 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var eqipmentService = new EquipmentService(dbContext);
            var documentFactory = new DocumentFactory(dbContext, eqipmentService);

            var request = new DocumentCreateRequest
            {
                RegistrationDate = new DateTime(2024, 1, 1),
                RepairDate = new DateTime(2024, 1, 1),
                DivisionId = Guid.NewGuid(),
                DocumentTypeId = Guid.NewGuid(),
                RepairFacilityId = Guid.NewGuid(),
                KKSEquipment = new List<KKSEquipmentRequest>() { new KKSEquipmentRequest("20KAA22AA366 20KAA21AA335", "Клапан запорный", "НГ-2265") },
                PerfomersId = new List<Guid> { Guid.NewGuid() },
                ExecuteRepairDocumentsId = null!
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessLogicException>(() => documentFactory.CreateDocumentAsync(request, CancellationToken.None));
        }
    }
}
