using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.DBContext
{
    public class DbContextFactory
    {
        public AppDbContext Create()
        {
            var builder = new DbContextOptionsBuilder().UseSqlite($"Data Source=DbContextApp.db");
            var dbContext = new AppDbContext(builder.Options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            return dbContext;
        }
    }
}
