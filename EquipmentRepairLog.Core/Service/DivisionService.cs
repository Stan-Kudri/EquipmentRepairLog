using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.ValidationData;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions.AppException;
using System.Data.Entity;

namespace EquipmentRepairLog.Core.Service
{
    public class DivisionService(AppDbContext dbContext)
    {
        public void Add(Division division)
        {
            ArgumentNullException.ThrowIfNull(division);

            if (dbContext.Divisions.FirstOrDefault(e => e.Abbreviation == division.Abbreviation
                                                        || e.Name == division.Name
                                                        || e.Number == division.Number) != null)
            {
                throw new DataTransferException($"Division \"{division.Name}\" have already been add to the app (DB).");
            }

            var divisionValidation = new DivisionValidation(division);

            dbContext.Divisions.Add(divisionValidation.Value);
            dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = dbContext.Divisions.FirstOrDefault(e => e.Id == id)
                        ?? throw new DataTransferException($"The ID for the division with \"{id}\" is already taken.");

            dbContext.Divisions.Remove(item);
            dbContext.SaveChanges();
        }

        public Division? GetDivision(Guid id)
            => dbContext.Divisions.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public Division? GetDivision(int number)
            => dbContext.Divisions.AsNoTracking().FirstOrDefault(e => e.Number == number);
    }
}
