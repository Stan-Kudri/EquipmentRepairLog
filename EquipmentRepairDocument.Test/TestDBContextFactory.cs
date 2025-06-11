using EquipmentRepairDocument.Core.DBContext;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Test
{
    public class TestDBContextFactory
    {
        public async Task<AppDbContext> Create()
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var sqlLiteConnection = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2000 // Dispose objects before losing scope

            await sqlLiteConnection.OpenAsync();
            var dbContext = new AppDbContext(new DbContextOptionsBuilder().UseSqlite(sqlLiteConnection).Options);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }
    }
}
