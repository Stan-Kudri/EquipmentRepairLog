using EquipmentRepairDocument.Core.Data.EquipmentModel;
using EquipmentRepairDocument.Core.Data.ValidationData;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Service;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class EquipmentServiceTest
    {
        public static IEnumerable<object[]> GetEquipmentData() =>
        [
            [
                new KKSEquipmentRequest
                {
                    KKS = "10KAA22AA345",
                    Equipment = "Клапан запорный",
                    EquipmentType = "КПЛВ.49833-12",
                }
            ]
        ];

        [Theory]
        [MemberData(nameof(GetEquipmentData))]
        public async Task AddEquipmentAsync_Should_Save_KKS_Equipment_To_Database(KKSEquipmentRequest kksEquipment)
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            // Act
            await equipmentService.AddEquipmentAsync(kksEquipment);
            var actualKKSEquipment = await dbContext.KKSEquipments.FirstOrDefaultAsync(e => e.KKS == kksEquipment.KKS);

            // Assert
            Assert.NotNull(actualKKSEquipment);
            Assert.Equal(kksEquipment.KKS, actualKKSEquipment.KKS);
        }

        [Fact(DisplayName = "AddEquipmentAsync with null equipment should throw exception")]
        public async Task AddEquipmentAsync_Null_Equipment_Throws_BusinessLogicException()
        {
            // Act & Assert            
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            await equipmentService.Invoking(s => s.AddEquipmentAsync(null!))
                                  .Should()
                                  .ThrowAsync<BusinessLogicException>();
        }

        [Fact(DisplayName = "AddEquipmentAsync with null Equipment property should throw exception")]
        public async Task AddEquipmentAsync_Null_Equipment_Property_Throws_BusinessLogicException()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = null!, // intentionally null
                EquipmentType = "Type",
                KKS = "GoodKKS"
            };

            // Act & Assert
            await equipmentService.Invoking(e => e.AddEquipmentAsync(kksEquipment))
                                  .Should()
                                  .ThrowAsync<BusinessLogicException>();
        }

        [Fact(DisplayName = "AddEquipmentAsync with null EquipmentType property should throw exception")]
        public async Task AddEquipmentAsync_Null_EquipmentType_Property_Throws_BusinessLogicException()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = "Equipment",
                EquipmentType = null!, // intentionally null
                KKS = "GoodKKS"
            };

            // Act & Assert
            await equipmentService.Invoking(s => s.AddEquipmentAsync(kksEquipment))
                          .Should().ThrowAsync<BusinessLogicException>();
        }

        [Theory(DisplayName = "AddEquipmentAsync with null or empty KKS should throw exception")]
        [InlineData(null)]
        [InlineData("")]
        public async Task AddEquipmentAsync_Invalid_KKS_Throws_BusinessLogicException(string? kks)
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = "Equipment",
                EquipmentType = "Type",
                KKS = kks // null or empty value
            };

            // Act & Assert
            await equipmentService.Invoking(s => s.AddEquipmentAsync(kksEquipment))
                                  .Should()
                                  .ThrowAsync<BusinessLogicException>();
        }

        [Fact(DisplayName = "AddEquipmentAsync with error in KKS parsing should throw exception")]
        public async Task AddEquipmentAsync_KKS_Parsing_Error_Throws_BusinessLogicException()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = "Equipment",
                EquipmentType = "Type",
                KKS = "error" // triggers an error result in KKS.CreateArray
            };

            // Act & Assert
            await equipmentService.Invoking(e => e.AddEquipmentAsync(kksEquipment))
                                  .Should()
                                  .ThrowAsync<BusinessLogicException>();
        }

        [Fact(DisplayName = "AddEquipmentAsync with valid input should add records to database")]
        public async Task AddEquipmentAsync_Valid_Input_Add_Records()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var equipmentName = "Equipment";
            var equipmentTypeName = "Type";
            var kksValue = "10KAA22AA345";

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = equipmentName,
                EquipmentType = equipmentTypeName,
                KKS = kksValue
            };

            // Act
            await equipmentService.AddEquipmentAsync(kksEquipment);

            // Assert - Check that the Equipment was added.
            var equipments = await dbContext.Equipments.ToListAsync();
            equipments.Should().HaveCount(1);
            equipments.First().Name.Should().Be(equipmentName);

            // Assert - Check that the EquipmentType was added and linked.
            var equipmentTypes = await dbContext.EquipmentTypes.ToListAsync();
            equipmentTypes.Should().HaveCount(1);
            equipmentTypes.First().Name.Should().Be(equipmentTypeName);
            equipmentTypes.First().EquipmentId.Should().Be(equipments.First().Id);

            // Assert - Check that KKSEquipment record was added.
            var kksequipments = await dbContext.KKSEquipments.ToListAsync();
            kksequipments.Should().HaveCount(1);
            kksequipments.First().KKS.Should().Be(kksValue);
            kksequipments.First().EquipmentId.Should().Be(equipments.First().Id);
            kksequipments.First().EquipmentTypeId.Should().Be(equipmentTypes.First().Id);
        }

        [Fact(DisplayName = "AddRangeEquipmentAsync with valid inputs should add records to database")]
        public async Task AddRangeEquipmentAsync_Valid_Inputs_Add_Records()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var equipmentFirst = new Equipment { Name = "EquipmentFirst", Id = Guid.NewGuid() };
            var equipmentSecond = new Equipment { Name = "EquipmentSecond", Id = Guid.NewGuid() };

            var list = new List<KKSEquipmentRequest>
            {
                new KKSEquipmentRequest
                {
                    Equipment = "EquipmentFirst",
                    EquipmentType = "TypeFirst",
                    KKS = "10KAA22AA341"
                },
                new KKSEquipmentRequest
                {
                    Equipment = "EquipmentSecond",
                    EquipmentType = "TypeSecond",
                    KKS = "10KAA22AA345"
                }
            };

            // Act
            await equipmentService.AddRangeEquipmentAsync(list);

            // Assert - Two unique Equipments.
            var equipments = await dbContext.Equipments.ToListAsync();
            equipments.Should().HaveCount(2);

            // Assert - Two EquipmentTypes.
            var equipmentTypes = await dbContext.EquipmentTypes.ToListAsync();
            equipmentTypes.Should().HaveCount(2);

            // Assert - Two KKSEquipments.
            var kksequipments = await dbContext.KKSEquipments.ToListAsync();
            kksequipments.Should().HaveCount(2);
        }

        [Fact(DisplayName = "AddRangeEquipmentAsync with a null list should throw exception")]
        public async Task AddRangeEquipmentAsync_Null_List_Throws_BusinessLogicException()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            await equipmentService.Invoking(s => s.AddRangeEquipmentAsync(null!))
                          .Should().ThrowAsync<BusinessLogicException>();
        }

        [Fact(DisplayName = "Duplicate AddEquipmentAsync calls should not create duplicate KKSEquipment records")]
        public async Task AddEquipmentAsync_Duplicate_Call_Does_Not_Duplicate_KKSEquipment()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            var kksValue = "10KAA22AA545";

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = "EquipmentDup",
                EquipmentType = "TypeDup",
                KKS = kksValue
            };

            // Act - Call the same addition twice.
            await equipmentService.AddEquipmentAsync(kksEquipment);
            await equipmentService.AddEquipmentAsync(kksEquipment);

            // Assert - KKSEquipments should contain only one record.
            var kksequipments = await dbContext.KKSEquipments.ToListAsync();
            kksequipments.Should().HaveCount(1);

            // Also, verify that Equipment and EquipmentType are not duplicated.
            var equipments = await dbContext.Equipments.ToListAsync();
            equipments.Should().HaveCount(1);
            var equipmentTypes = await dbContext.EquipmentTypes.ToListAsync();
            equipmentTypes.Should().HaveCount(1);
        }
    }
}
