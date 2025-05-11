using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;
using System.Data.Entity;

namespace EquipmentRepairLog.Core.Service
{
    public class ServiceDivision(AppDbContext dbContext)
    {
        public void Add(Division division)
        {
            ArgumentNullException.ThrowIfNull(division);

            if (dbContext.Divisions.FirstOrDefault(e => e.Abbreviation == division.Abbreviation
                                                        || e.Name == division.Name
                                                        || e.Number == division.Number) != null)
            {
                throw new ArgumentException("Data already in use.", nameof(division));
            }

            dbContext.Divisions.Add(division);
            dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = dbContext.Divisions.FirstOrDefault(e => e.Id == id)
                        ?? throw new InvalidOperationException("Interaction element not found.");

            dbContext.Divisions.Remove(item);
            dbContext.SaveChanges();
        }

        public Division? GetDivision(Guid id)
            => dbContext.Divisions.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public Division? GetDivision(int number)
            => dbContext.Divisions.AsNoTracking().FirstOrDefault(e => e.Number == number);
    }
}
