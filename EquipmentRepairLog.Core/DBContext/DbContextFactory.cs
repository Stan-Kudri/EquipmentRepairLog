using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.DBContext
{
    public class DbContextFactory
    {
        public async Task<AppDbContext> Create()
        {
            var builder = new DbContextOptionsBuilder().UseSqlite($"Data Source=DbContextApp.db");
            var dbContext = new AppDbContext(builder.Options);
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }
    }
}
