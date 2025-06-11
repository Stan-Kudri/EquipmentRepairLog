using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Test
{
    public class TestDBContextFactory
    {
        public static async Task<TContext> Create<TContext>()
            where TContext : DbContext
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var sqlLiteConnection = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2000 // Dispose objects before losing scope

            await sqlLiteConnection.OpenAsync();
            var dbContextBuilder = new DbContextOptionsBuilder<TContext>().EnableDetailedErrors().UseSqlite(sqlLiteConnection);
            var dbContext = (TContext?)Activator.CreateInstance(typeof(TContext), dbContextBuilder.Options) ?? throw new InvalidOperationException($"Unable to create {typeof(TContext).Name}");
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }
    }
}
