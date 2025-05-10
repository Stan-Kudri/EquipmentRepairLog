using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;

namespace EquipmentRepairLog.Core.Service
{
    public class PerfomerService
    {
        private readonly AppDbContext _dbContext;

        public PerfomerService(AppDbContext dbContext)
            => _dbContext = dbContext;

        public void Add(Perfomer perfomer)
        {
            ArgumentNullException.ThrowIfNull(perfomer);

            if (_dbContext.Perfomers.FirstOrDefault(e => e.Abbreviation == perfomer.Abbreviation
                                                        || e.Name == perfomer.Name) != null)
            {
                throw new ArgumentException("Data already in use.", nameof(perfomer));
            }

            _dbContext.Perfomers.Add(perfomer);
            _dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = _dbContext.Perfomers.FirstOrDefault(e => e.Id == id)
                        ?? throw new InvalidOperationException("Interaction element not found.");

            _dbContext.Perfomers.Remove(item);
            _dbContext.SaveChanges();
        }

        public Perfomer? GetPerfomer(Guid id)
            => _dbContext.Perfomers.FirstOrDefault(e => e.Id == id);

        public Perfomer? GetPerfomer(string abbreviation)
            => _dbContext.Perfomers.FirstOrDefault(e => e.Abbreviation == abbreviation);
    }
}
