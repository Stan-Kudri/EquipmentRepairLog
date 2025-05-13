using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.ValidationData;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions.AppException;

namespace EquipmentRepairLog.Core.Service
{
    public class PerfomerService(AppDbContext dbContext, PerfomerFactory perfomerFactory)
    {
        public void Add(Perfomer perfomer)
        {
            ArgumentNullException.ThrowIfNull(perfomer);

            if (dbContext.Perfomers.Any(e => e.Abbreviation == perfomer.Abbreviation || e.Name == perfomer.Name))
            {
                throw new DataTransferException($"Performers of the works \"{perfomer.Name}\" have already been add to the app (DB).");
            }

            var perfomerNormalize = perfomerFactory.Create(perfomer.Name, perfomer.Abbreviation);
            dbContext.Perfomers.Add(perfomerNormalize);
            dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = dbContext.Perfomers.FirstOrDefault(e => e.Id == id)
                        ?? throw new DataTransferException($"The ID for the perfomer with \"{id}\" is already taken.");

            dbContext.Perfomers.Remove(item);
            dbContext.SaveChanges();
        }

        public Perfomer? GetPerfomer(Guid id)
            => dbContext.Perfomers.FirstOrDefault(e => e.Id == id);

        public Perfomer? GetPerfomer(string abbreviation)
            => dbContext.Perfomers.FirstOrDefault(e => e.Abbreviation == abbreviation);
    }
}
