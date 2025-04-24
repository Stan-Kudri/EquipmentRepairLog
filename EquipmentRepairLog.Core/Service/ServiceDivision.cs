using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;

namespace EquipmentRepairLog.Core.Service
{
    public class ServiceDivision
    {
        private readonly AppDbContext _dbContext;

        public ServiceDivision(AppDbContext dbContext)
            => _dbContext = dbContext;

        public void Add(Division division)
        {
            if (division == null)
            {
                throw new ArgumentNullException("Transmitted data error.", nameof(division));
            }
            if (_dbContext.Divisions.FirstOrDefault(e => e.Abbreviation == division.Abbreviation
                                                        || e.Name == division.Name
                                                        || e.Number == division.Number) != null)
            {
                throw new ArgumentException("Data already in use.", nameof(division));
            }
            if (_dbContext.Divisions.FirstOrDefault(e => e.Id == division.Id) != null)
            {
                division.Id = ChangeIdDivision();
            }

            _dbContext.Divisions.Add(division);
            _dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = _dbContext.Divisions.FirstOrDefault(e => e.Id == id)
                        ?? throw new InvalidOperationException("Interaction element not found.");

            _dbContext.Divisions.Remove(item);
            _dbContext.SaveChanges();
        }

        public Division? GetDivision(Guid id)
            => _dbContext.Divisions.FirstOrDefault(e => e.Id == id);

        public Division? GetDivision(int number)
            => _dbContext.Divisions.FirstOrDefault(e => e.Number == number);

        private Guid ChangeIdDivision()
        {
            var id = Guid.NewGuid();

            return _dbContext.Divisions.FirstOrDefault(d => d.Id == id) == null ? id : ChangeIdDivision();
        }
    }
}
