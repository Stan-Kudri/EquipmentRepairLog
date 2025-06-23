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
            dbContext.ChangeTracker.Clear();
            var actualKKSEquipment = await dbContext.KKSEquipments.FirstOrDefaultAsync(e => e.KKS == kksEquipment.KKS);

            // Assert
            Assert.NotNull(actualKKSEquipment);
            Assert.Equal(kksEquipment.KKS, actualKKSEquipment.KKS);
        }

        [Fact(DisplayName = "AddEquipmentAsync with null equipment should throw BusinessLogicException")]
        public async Task AddEquipmentAsync_Null_Equipment_Throws_BusinessLogicException()
        {
            // Act & Assert            
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);
            var error = await Assert.ThrowsAnyAsync<BusinessLogicException>(() => equipmentService.AddEquipmentAsync(null!));
            error.Message.Should().Be(BusinessLogicException.MessageEmptyObject);
        }

        [Fact(DisplayName = "AddEquipmentAsync with null Equipment property should throw BusinessLogicException")]
        public async Task AddEquipmentAsync_Null_Equipment_Property_Throws_BusinessLogicException()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = null!,
                EquipmentType = "Type",
                KKS = "GoodKKS"
            };

            // Act & Assert
            var error = await Assert.ThrowsAnyAsync<BusinessLogicException>(() => equipmentService.AddEquipmentAsync(kksEquipment));
            error.Message.Should().Be(BusinessLogicException.MessageNullOrEmptyStr);
        }

        [Fact(DisplayName = "AddEquipmentAsync with null EquipmentType property should throw BusinessLogicException")]
        public async Task AddEquipmentAsync_Null_EquipmentType_Property_Throws_BusinessLogicException()
        {
            // Arrange                 
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            var kksEquipment = new KKSEquipmentRequest
            {
                Equipment = "Equipment",
                EquipmentType = null!,
                KKS = "GoodKKS"
            };

            // Act & Assert                                              
            var error = await Assert.ThrowsAnyAsync<BusinessLogicException>(() => equipmentService.AddEquipmentAsync(kksEquipment));
            error.Message.Should().Be(BusinessLogicException.MessageNullOrEmptyStr);
        }

        [Theory(DisplayName = "AddEquipmentAsync with null or empty KKS should throw BusinessLogicException")]
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
                KKS = kks,
            };

            // Act & Assert
            var error = await Assert.ThrowsAnyAsync<BusinessLogicException>(() => equipmentService.AddEquipmentAsync(kksEquipment));
            error.Message.Should().Be(BusinessLogicException.MessageNullOrEmptyStr);
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
                KKS = "error",
            };

            // Act & Assert
            await Assert.ThrowsAnyAsync<BusinessLogicException>(() => equipmentService.AddEquipmentAsync(kksEquipment));
            /*
            await equipmentService.Invoking(e => e.AddEquipmentAsync(kksEquipment))
                                  .Should()
                                  .ThrowAsync<BusinessLogicException>();*/
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
            dbContext.ChangeTracker.Clear();

            // Assert
            // Проверка добавления элементов
            var equipment = await dbContext.Equipments.SingleAsync();
            var equipmentType = await dbContext.EquipmentTypes.SingleAsync();
            var kksEquipments = await dbContext.KKSEquipments.SingleAsync();

            // Сгруппированные проверки
            equipment.Name.Should().Be(equipmentName);

            equipmentType.Name.Should().Be(equipmentTypeName);
            equipmentType.EquipmentId.Should().Be(equipment.Id);

            kksEquipments.KKS.Should().Be(kksValue);
            kksEquipments.EquipmentId.Should().Be(equipment.Id);
            kksEquipments.EquipmentTypeId.Should().Be(equipmentType.Id);
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
            dbContext.ChangeTracker.Clear();

            // Assert

            // Two unique Equipments.
            var equipments = await dbContext.Equipments.ToListAsync();
            equipments.Should().HaveCount(2);

            // Two EquipmentTypes.
            var equipmentTypes = await dbContext.EquipmentTypes.ToListAsync();
            equipmentTypes.Should().HaveCount(2);

            // Two KKSEquipments.
            var kksEquipments = await dbContext.KKSEquipments.ToListAsync();
            kksEquipments.Should().HaveCount(2);
        }

        [Fact(DisplayName = "AddRangeEquipmentAsync with a null list should throw exception")]
        public async Task AddRangeEquipmentAsync_Null_List_Throws_BusinessLogicException()
        {
            // Arrange
            var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            // Act & Assert
            var error = await Assert.ThrowsAnyAsync<BusinessLogicException>(() => equipmentService.AddEquipmentAsync(null!));
            error.Message.Should().Be(BusinessLogicException.MessageEmptyObject);
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
                KKS = kksValue,
            };

            await equipmentService.AddEquipmentAsync(kksEquipment);
            await equipmentService.AddEquipmentAsync(kksEquipment);
            dbContext.ChangeTracker.Clear();

            // Assert
            // Во всех трёх таблицах по одному элементу (Нет дубликата)
            var equipmentCount = await dbContext.KKSEquipments.CountAsync();
            var kksCount = await dbContext.Equipments.CountAsync();
            var typeEquipmentCount = await dbContext.EquipmentTypes.CountAsync();

            equipmentCount.Should().Be(1);
            kksCount.Should().Be(1);
            typeEquipmentCount.Should().Be(1);
        }
    }
}
