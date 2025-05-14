using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.Users;
using EquipmentRepairLog.Core.DBContext.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(DivisionConfiguration).Assembly);
    }
}
