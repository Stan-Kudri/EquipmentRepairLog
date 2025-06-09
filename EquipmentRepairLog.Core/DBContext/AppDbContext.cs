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

        public DbSet<User> Users { get; set; }

        public DbSet<Division> Divisions { get; set; }

        public DbSet<DocumentType> DocumentTypes { get; set; }

        public DbSet<Perfomer> Perfomers { get; set; }

        public DbSet<RepairFacility> RepairFacilities { get; set; }

        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<EquipmentType> EquipmentTypes { get; set; }

        public DbSet<KKSEquipment> KKSEquipments { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<ExecuteRepairDocument> ExecuteRepairDocuments { get; set; }

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
