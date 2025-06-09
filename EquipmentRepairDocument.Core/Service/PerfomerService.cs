using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Exceptions.AppException;
using EquipmentRepairDocument.Core.FactoryData;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Core.Service
{
    public class PerfomerService(AppDbContext dbContext, PerfomerFactory perfomerFactory)
    {
        public async Task AddAsync(Perfomer perfomer, CancellationToken cancellationToken = default)
        {
            BusinessLogicException.ThrowIfNull(perfomer);

            var existingPerfomer = dbContext.Perfomers.FirstOrDefault(e => e.Abbreviation == perfomer.Abbreviation || e.Name == perfomer.Name);

            if (existingPerfomer == null)
            {
                var perfomerNormalize = perfomerFactory.Create(perfomer.Name, perfomer.Abbreviation);
                await dbContext.Perfomers.AddAsync(perfomerNormalize, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
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

        public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var count = await dbContext.Perfomers.Where(e => e.Id == id).ExecuteDeleteAsync(cancellationToken);
            if (count == 0)
            {
                throw new NotFoundException($"The ID for the perfomer with \"{id}\" is already taken.");
            }
        }
    }
}
