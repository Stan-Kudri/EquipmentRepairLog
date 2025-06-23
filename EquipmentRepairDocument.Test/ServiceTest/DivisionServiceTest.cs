using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.FactoryData;
using EquipmentRepairDocument.Core.Service;
using FluentAssertions;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class DivisionServiceTest
    {
        public static IEnumerable<object[]> DivisionItems() => new List<object[]>
        {
            new object[]
            {
                new List<Division>()
                {
                    new DivisionFactory().Create("Отдел подготовки и проведения ремонтов", "ОППР", 38),
                    new DivisionFactory().Create("Реакторный цех", "РЦ", 21),
                    new DivisionFactory().Create("Турбинный цех", "ТЦ", 22),
                },
            }
        };

        public static IEnumerable<object[]> AddItemDivision() => new List<object[]>
        {
            new object[]
            {
                new List<Division>()
                {
                    new DivisionFactory().Create("Химический цех", "ХЦ", 25),
                    new DivisionFactory().Create("Цех вентиляции и кондиционирования", "ЦВиК", 26),
                },
                new DivisionFactory().Create("Турбинный цех", "ТЦ", 22),
                new List<Division>()
                {
                    new DivisionFactory().Create("Химический цех", "ХЦ", 25),
                    new DivisionFactory().Create("Цех вентиляции и кондиционирования", "ЦВиК", 26),
                    new DivisionFactory().Create("Турбинный цех", "ТЦ", 22),
                },
            },
        };

        public static IEnumerable<object[]> RemoveItemDivision() => new List<object[]>
        {
            new object[]
            {
                new List<Division>()
                {
                    new DivisionFactory().Create("Химический цех", "ХЦ", 25),
                    new DivisionFactory().Create("Цех вентиляции и кондиционирования", "ЦВиК", 26),
                    new DivisionFactory().Create("Турбинный цех", "ТЦ", 22),
                },
                22,
                new List<Division>()
                {
                    new DivisionFactory().Create("Химический цех", "ХЦ", 25),
                    new DivisionFactory().Create("Цех вентиляции и кондиционирования", "ЦВиК", 26),
                },
            },
        };

        [Theory]
        [MemberData(nameof(DivisionItems))]
        public async Task Service_Should_Add_All_The_Item_Of_Database(List<Division> division)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var divisionFactory = new DivisionFactory();
            var divisionService = new DivisionService(dbContext, divisionFactory);
            await divisionService.AddRangeAsync(division);
            dbContext.ChangeTracker.Clear();

            // Act
            var actualDivisions = dbContext.Divisions.ToList();

            // Assert
            actualDivisions.Should().Equal(division);
        }

        [Theory]
        [MemberData(nameof(AddItemDivision))]
        public async Task Service_Should_Add_The_Item_To_The_Database(List<Division> divisions, Division addDivision, List<Division> expectDivisions)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var divisionFactory = new DivisionFactory();
            var divisionService = new DivisionService(dbContext, divisionFactory);
            await divisionService.AddRangeAsync(divisions);
            await divisionService.AddAsync(addDivision);
            dbContext.ChangeTracker.Clear();

            // Act
            var actualDivisions = dbContext.Divisions.ToList();

            // Assert
            actualDivisions.Should().Equal(expectDivisions);
        }

        [Theory]
        [MemberData(nameof(RemoveItemDivision))]
        public async Task Service_Should_Remove_Item_By_Number_Division_To_The_Database(List<Division> divisions, byte removeByNumber, List<Division> expectDivisions)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var divisionFactory = new DivisionFactory();
            var divisionService = new DivisionService(dbContext, divisionFactory);
            await divisionService.AddRangeAsync(divisions);
            await divisionService.RemoveAsync(removeByNumber);
            dbContext.ChangeTracker.Clear();

            // Act
            var actualDivisions = dbContext.Divisions.ToList();

            // Assert
            actualDivisions.Should().Equal(expectDivisions);
        }
    }
}
