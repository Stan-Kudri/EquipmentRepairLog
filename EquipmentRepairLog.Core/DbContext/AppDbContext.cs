using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.Users;
using EquipmentRepairLog.Core.DBContext.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EquipmentRepairLog.Core.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Division> Divisions { get; set; } = null!;

        public DbSet<DocumentType> DocumentTypes { get; set; } = null!;

        public DbSet<Perfomer> Perfomers { get; set; } = null!;

        public DbSet<RepairFacility> RepairFacilities { get; set; } = null!;

        public DbSet<Equipment> Equipments { get; set; } = null!;

        public DbSet<EquipmentType> EquipmentTypes { get; set; } = null!;

        public DbSet<KKSEquipment> KKSEquipments { get; set; } = null!;

        public DbSet<Document> Documents { get; set; } = null!;

        public DbSet<ExecuteRepairDocument> ExecuteRepairDocuments { get; set; } = null!;

        /// <summary>
        /// Runs code within a transaction, taking into account previously created code.
        /// </summary>
        /// <param name="action">Action for transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <typeparam name="TRes">Result type.</typeparam>
        /// <returns>Representing the asynchronous operation.</returns>
        public async Task<TRes> RunTransactionAsync<TRes>(
            Func<CancellationToken, Task<TRes>> action,
            CancellationToken cancellationToken)
        {
            using var transaction = Database.CurrentTransaction is not null
                                    ? EmptyDisposable.Instance
                                    : await Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await action(cancellationToken);
                if (transaction is IDbContextTransaction typedTransaction)
                {
                    await typedTransaction.CommitAsync(cancellationToken);
                }

                return result;
            }
            catch (Exception)
            {
                if (transaction is IDbContextTransaction typedTransaction)
                {
                    await typedTransaction.RollbackAsync(cancellationToken);
                }

                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(DivisionConfiguration).Assembly);
    }
}
