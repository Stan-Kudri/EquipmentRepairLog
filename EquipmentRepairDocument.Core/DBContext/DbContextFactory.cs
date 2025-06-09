using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Core.DBContext
{
    public class DbContextFactory
    {
        public async Task<AppDbContext> CreateAsync()
        {
            var builder = new DbContextOptionsBuilder().UseSqlite($"Data Source=DbContextApp.db");
            var dbContext = new AppDbContext(builder.Options);
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }
    }
}
