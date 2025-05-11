using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;

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
                throw new ArgumentException("Data already in use.", nameof(perfomer));
            }

            dbContext.Perfomers.Add(perfomer);
            dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = dbContext.Perfomers.FirstOrDefault(e => e.Id == id)
                        ?? throw new InvalidOperationException("Interaction element not found.");

            dbContext.Perfomers.Remove(item);
            dbContext.SaveChanges();
        }

        public Perfomer? GetPerfomer(Guid id)
            => dbContext.Perfomers.FirstOrDefault(e => e.Id == id);

        public Perfomer? GetPerfomer(string abbreviation)
            => dbContext.Perfomers.FirstOrDefault(e => e.Abbreviation == abbreviation);
    }
}
