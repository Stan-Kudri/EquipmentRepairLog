using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.FactoryData;
using EquipmentRepairDocument.Core.Service;
using FluentAssertions;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class DocumentTypeServiceTest
    {

        public static IEnumerable<object[]> DocumentTypeItems() => new List<object[]>
        {
            new object[]
            {
                new List<DocumentType>()
                {
                    new DocumentTypeFactory().Create("Акт о выполненных работах по ремонту оборудования", "АВР", 23, true),
                    new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 27, true),
                },
            }
        };

        public static IEnumerable<object[]> AddItemDocumentType() => new List<object[]>
        {
            new object[]
            {
                new List<DocumentType>()
                {
                    new DocumentTypeFactory().Create("Акт о выполненных работах по ремонту оборудования", "АВР", 23, true),
                    new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 27, true),
                },
                new DocumentTypeFactory().Create("Протоколы закрытия оборудования", "ПЗО", 24, true),
                new List<DocumentType>()
                {
                    new DocumentTypeFactory().Create("Акт о выполненных работах по ремонту оборудования", "АВР", 23, true),
                    new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 27, true),
                    new DocumentTypeFactory().Create("Протоколы закрытия оборудования", "ПЗО", 24, true),
                },
            },
        };

        public static IEnumerable<object[]> RemoveItemDocumentType() => new List<object[]>
        {
            new object[]
            {
                new List<DocumentType>()
                {
                    new DocumentTypeFactory().Create("Акт о выполненных работах по ремонту оборудования", "АВР", 23, true),
                    new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 27, true),
                    new DocumentTypeFactory().Create("Протоколы закрытия оборудования", "ПЗО", 24, true),
                },
                24,
                new List<DocumentType>()
                {
                    new DocumentTypeFactory().Create("Акт о выполненных работах по ремонту оборудования", "АВР", 23, true),
                    new DocumentTypeFactory().Create("Ведомость выполненных работ", "ВВР", 27, true),
                },
            },
        };

        [Theory]
        [MemberData(nameof(DocumentTypeItems))]
        public async Task Service_Should_Add_All_The_Item_Of_Database(List<DocumentType> documentTypes)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var documentTypeFactory = new DocumentTypeFactory();
            var documentTypeService = new DocumentTypeService(dbContext, documentTypeFactory);
            await documentTypeService.AddRangeAsync(documentTypes);
            dbContext.ChangeTracker.Clear();

            // Act
            var actualDocumentTypes = dbContext.DocumentTypes.ToList();

            // Assert
            actualDocumentTypes.Should().Equal(documentTypes);
        }

        [Theory]
        [MemberData(nameof(AddItemDocumentType))]
        public async Task Service_Should_Add_The_Item_To_The_Database(List<DocumentType> DocumentTypes, DocumentType addDocumentType, List<DocumentType> expectDocumentTypes)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var documentTypeFactory = new DocumentTypeFactory();
            var documentTypeService = new DocumentTypeService(dbContext, documentTypeFactory);
            await documentTypeService.AddRangeAsync(DocumentTypes);
            await documentTypeService.AddAsync(addDocumentType);
            dbContext.ChangeTracker.Clear();

            // Act                                              
            var actualDocumentTypes = dbContext.DocumentTypes.ToList();

            // Assert
            actualDocumentTypes.Should().Equal(expectDocumentTypes);
        }

        [Theory]
        [MemberData(nameof(RemoveItemDocumentType))]
        public async Task Service_Should_Remove_Item_By_Number_DocumentType_To_The_Database(List<DocumentType> DocumentTypes, byte number, List<DocumentType> expectDocumentTypes)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var documentTypeFactory = new DocumentTypeFactory();
            var documentTypeService = new DocumentTypeService(dbContext, documentTypeFactory);
            await documentTypeService.AddRangeAsync(DocumentTypes);
            await documentTypeService.RemoveAsync(number);
            dbContext.ChangeTracker.Clear();

            // Act                                              
            var actualDocumentTypes = dbContext.DocumentTypes.ToList();

            // Assert
            actualDocumentTypes.Should().Equal(expectDocumentTypes);
        }
    }
}
