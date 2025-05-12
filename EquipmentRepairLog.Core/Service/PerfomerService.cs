using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.ValidationData;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions.AppException;

namespace EquipmentRepairLog.Core.Service
{
    public class PerfomerService(AppDbContext dbContext)
    {
        public void Add(Perfomer perfomer)
        {
            ArgumentNullException.ThrowIfNull(perfomer);

            if (dbContext.Perfomers.FirstOrDefault(e => e.Abbreviation == perfomer.Abbreviation
                                                        || e.Name == perfomer.Name) != null)
            {
                throw new DataTransferException($"Performers of the works \"{perfomer.Name}\" have already been add to the app (DB).");
            }

            var perfomerValidation = new PerfomerValidation(perfomer);

            dbContext.Perfomers.Add(perfomerValidation.Value);
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
