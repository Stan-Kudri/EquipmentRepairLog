using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.FactoryData;
using EquipmentRepairDocument.Core.Service;
using FluentAssertions;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class PerfomerServiceTest
    {
        public static IEnumerable<object[]> PerfomerItems() => new List<object[]>
        {
            new object[]
            {
                new List<Perfomer>()
                {
                    new PerfomerFactory().Create("Электрический цех", "ЭЦ"),
                    new PerfomerFactory().Create("Белэнергомантажналадка", "БЭМН"),
                },
            }
        };

        public static IEnumerable<object[]> AddItemPerfomer() => new List<object[]>
        {
            new object[]
            {
                new List<Perfomer>()
                {
                    new PerfomerFactory().Create("Электрический цех", "ЭЦ"),
                    new PerfomerFactory().Create("Белэнергомантажналадка", "БЭМН"),
                },
                new PerfomerFactory().Create("Цех централизованного ремонта", "ЦЦР"),
                new List<Perfomer>()
                {
                    new PerfomerFactory().Create("Электрический цех", "ЭЦ"),
                    new PerfomerFactory().Create("Белэнергомантажналадка", "БЭМН"),
                    new PerfomerFactory().Create("Цех централизованного ремонта", "ЦЦР"),
                },
            },
        };

        public static IEnumerable<object[]> RemoveItemPerfomer() => new List<object[]>
        {
            new object[]
            {
                new List<Perfomer>()
                {
                    new PerfomerFactory().Create("Электрический цех", "ЭЦ"),
                    new PerfomerFactory().Create("Белэнергомантажналадка", "БЭМН"),
                    new PerfomerFactory().Create("Цех централизованного ремонта", "ЦЦР"),
                },
                "ЦЦР",
                new List<Perfomer>()
                {
                    new PerfomerFactory().Create("Электрический цех", "ЭЦ"),
                    new PerfomerFactory().Create("Белэнергомантажналадка", "БЭМН"),
                },
            },
        };

        [Theory]
        [MemberData(nameof(PerfomerItems))]
        public async Task Service_Should_Add_All_The_Item_Of_Database(List<Perfomer> perfomers)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var perfomerFactory = new PerfomerFactory();
            var perfomerService = new PerfomerService(dbContext, perfomerFactory);
            await perfomerService.AddRangeAsync(perfomers);
            dbContext.ChangeTracker.Clear();

            // Act
            var actualPerfomers = dbContext.Perfomers.ToList();

            // Assert
            actualPerfomers.Should().Equal(perfomers);
        }

        [Theory]
        [MemberData(nameof(AddItemPerfomer))]
        public async Task Service_Should_Add_The_Item_To_The_Database(List<Perfomer> perfomers, Perfomer addPerfomer, List<Perfomer> expectPerfomers)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var perfomerFactory = new PerfomerFactory();
            var perfomerService = new PerfomerService(dbContext, perfomerFactory);
            await perfomerService.AddRangeAsync(perfomers);
            await perfomerService.AddAsync(addPerfomer);
            dbContext.ChangeTracker.Clear();

            // Act                                              
            var actualPerfomers = dbContext.Perfomers.ToList();

            // Assert
            actualPerfomers.Should().Equal(expectPerfomers);
        }

        [Theory]
        [MemberData(nameof(RemoveItemPerfomer))]
        public async Task Service_Should_Remove_Item_By_Abbreviation_Perfomer_To_The_Database(List<Perfomer> perfomers, string removeByAbbreviation, List<Perfomer> expectPerfomers)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var perfomerFactory = new PerfomerFactory();
            var perfomerService = new PerfomerService(dbContext, perfomerFactory);
            await perfomerService.AddRangeAsync(perfomers);
            await perfomerService.RemoveAsync(removeByAbbreviation);
            dbContext.ChangeTracker.Clear();

            // Act                                                
            var actualPerfomers = dbContext.Perfomers.ToList();

            // Assert
            actualPerfomers.Should().Equal(expectPerfomers);
        }
    }
}
