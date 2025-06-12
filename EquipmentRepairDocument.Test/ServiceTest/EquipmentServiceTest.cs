using EquipmentRepairDocument.Core.Data.EquipmentModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Service;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class EquipmentServiceTest
    {
        public static IEnumerable<object[]> GetEquipmentData() =>
        [
            [
                new KKSEquipment
                {
                    Id = new Guid("AFF3C087-5F43-4266-8342-542A8A39B766"),
                    KKS = "10KAA22AA345",
                    EquipmentId = new Guid("ACC3C087-5F43-4266-8342-542A8A39B766"),
                    EquipmentTypeId = new Guid("AAC3C087-5F43-4266-8342-542A8A39B766"),
                    Equipment = new Equipment
                    {
                        Id = new Guid("ACC3C087-5F43-4266-8342-542A8A39B766"),
                        Name = "Клапан запорный",
                        Description = "Клапан новый"
                    },
                    EquipmentType = new EquipmentType
                    {
                        Id = new Guid("AAC3C087-5F43-4266-8342-542A8A39B766"),
                        EquipmentId = new Guid("ACC3C087-5F43-4266-8342-542A8A39B766"),
                        Name = "КПЛВ.49833-12"
                    }
                }
            ]
        ];

        [Theory]
        [MemberData(nameof(GetEquipmentData))]
        public async Task AddEquipmentAsync_Should_Save_KKS_Equipment_To_Database(KKSEquipment kksEquipment)
        {
            // Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var equipmentService = new EquipmentService(dbContext);

            // Act
            await equipmentService.AddEquipmentAsync(kksEquipment);
            var actualKKSEquipment = await dbContext.KKSEquipments.FirstOrDefaultAsync(e => e.KKS == kksEquipment.KKS);

            // Assert
            Assert.NotNull(actualKKSEquipment);
            Assert.Equal(kksEquipment.KKS, actualKKSEquipment.KKS);
            Assert.Equal(kksEquipment.EquipmentId, actualKKSEquipment.EquipmentId);
            Assert.Equal(kksEquipment.EquipmentTypeId, actualKKSEquipment.EquipmentTypeId);
        }
    }
}
