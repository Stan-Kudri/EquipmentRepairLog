using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using EquipmentRepairLog.Core.FactoryData;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class PerfomerService(AppDbContext dbContext, PerfomerFactory perfomerFactory)
    {
        public async Task AddAsync(Perfomer perfomer)
        {
            BusinessLogicException.ThrowIfNull(perfomer);

            var existingPerfomer = dbContext.Perfomers.FirstOrDefault(e => e.Abbreviation == perfomer.Abbreviation || e.Name == perfomer.Name);

            if (existingPerfomer == null)
            {
                var perfomerNormalize = perfomerFactory.Create(perfomer.Name, perfomer.Abbreviation);
                await dbContext.Perfomers.AddAsync(perfomerNormalize);
                await dbContext.SaveChangesAsync();
                return;
            }

            if (existingPerfomer.Abbreviation == perfomer.Abbreviation)
            {
                BusinessLogicException.EnsureUniqueProperty<Division>(existingPerfomer.Abbreviation);
            }
            if (existingPerfomer.Name == perfomer.Name)
            {
                BusinessLogicException.EnsureUniqueProperty<Division>(existingPerfomer.Name);
            }
        }

        public async Task Remove(Guid id)
        {
            var count = await dbContext.Perfomers.Where(e => e.Id == id).ExecuteDeleteAsync();
            if (count == 0)
            {
                throw new NotFoundException($"The ID for the perfomer with \"{id}\" is already taken.");
            }
        }
    }
}
