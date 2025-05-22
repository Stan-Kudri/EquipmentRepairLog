using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.ValidationData;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;

namespace EquipmentRepairLog.Core.Service
{
    public class DivisionService(AppDbContext dbContext, DivisionFactory divisionFactory)
    {
        public void Add(Division division)
        {
            ArgumentNullException.ThrowIfNull(division);

            var existingDivision = dbContext.Divisions.FirstOrDefault(e => e.Abbreviation == division.Abbreviation
                                                                           || e.Name == division.Name
                                                                           || e.Number == division.Number);

            if (existingDivision != null)
            {
                if (existingDivision.Abbreviation == division.Abbreviation)
                {
                    throw new BusinessLogicException($"Division \"{division.Abbreviation}\" have already been add to the app (DB).");
                }
                if (existingDivision.Name == division.Name)
                {
                    throw new BusinessLogicException($"Division \"{division.Name}\" have already been add to the app (DB).");
                }
                if (existingDivision.Number == division.Number)
                {
                    throw new BusinessLogicException($"Division \"{division.Number}\" have already been add to the app (DB).");
                }
            }

            var divisionNormalize = divisionFactory.Create(division.Name, division.Abbreviation, division.Number);
            dbContext.Divisions.Add(divisionNormalize);
            dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = dbContext.Divisions.FirstOrDefault(e => e.Id == id)
                        ?? throw new NotFoundException($"The ID for the division with \"{id}\" is already taken.");

            dbContext.Divisions.Remove(item);
            dbContext.SaveChanges();
        }
    }
}
