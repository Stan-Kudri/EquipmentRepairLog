using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Division> Divisions { get; set; }

        public DbSet<DocumentType> DocumentTypes { get; set; }

        public DbSet<Perfomer> Perfomers { get; set; }

        public DbSet<RepairFacility> RepairFacilities { get; set; }

        public DbSet<Equipment> Equipments { get; set; }

        public DbSet<EquipmentType> EquipmentTypes { get; set; }

        public DbSet<KKSEquipment> KKSEquipments { get; set; }

        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Division>(builder =>
            {
                builder.ToTable("division");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
                builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
                builder.Property(x => x.Number).HasColumnName("number").IsRequired().HasColumnType("INTEGER");
                builder.HasMany(x => x.Documents).WithOne(x => x.Division).HasForeignKey(x => x.DivisionId);
            });

            modelBuilder.Entity<DocumentType>(builder =>
            {
                builder.ToTable("document_type");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
                builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
                builder.Property(x => x.IsOnlyTypeDocInRepairLog).HasColumnName("is_single_type_document").IsRequired().HasColumnType("BOOLEAN").HasDefaultValue(false);
                builder.HasMany(x => x.Documents).WithOne(x => x.DocumentType).HasForeignKey(x => x.DocumentTypeId);
            });

            modelBuilder.Entity<Perfomer>(builder =>
            {
                builder.ToTable("perfomer");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
                builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
                builder.HasMany(x => x.Documents).WithMany(x => x.Perfomers).UsingEntity(e => e.ToTable("perfomer_work_document"));
            });

            modelBuilder.Entity<RepairFacility>(builder =>
            {
                builder.ToTable("repair_facility");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
                builder.Property(x => x.Abbreviation).HasColumnName("abbreviation").IsRequired().HasMaxLength(32);
                builder.Property(x => x.Number).HasColumnName("number").IsRequired().HasColumnType("INTEGER");
                builder.HasMany(x => x.Documents).WithOne(x => x.RepairFacility).HasForeignKey(x => x.RepairFacilityId);
            });


            modelBuilder.Entity<Equipment>(builder =>
            {
                builder.ToTable("equipment");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
                builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(256).HasDefaultValue(string.Empty);
                builder.HasMany(x => x.EquipmentsKKS).WithOne(x => x.Equipment).HasForeignKey(x => x.EquipmentId);
            });

            modelBuilder.Entity<EquipmentType>(builder =>
            {
                builder.ToTable("equipment_type");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            });

            modelBuilder.Entity<KKSEquipment>(builder =>
            {
                builder.ToTable("kks_equipment");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.KKS).HasColumnName("kks").IsRequired().HasMaxLength(128);
                builder.HasOne(x => x.Equipment).WithMany(x => x.EquipmentsKKS);
                builder.HasMany(x => x.KKSEquipmentDocuments).WithMany(x => x.KKSEquipment).UsingEntity(e => e.ToTable("equipment_KKS_documents"));
            });

            modelBuilder.Entity<Document>(builder =>
            {
                builder.ToTable("document");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                builder.Property(x => x.RegistrationDate).IsRequired().HasColumnType("DATETIME").HasColumnName("date_registration").HasDefaultValue(DateTime.Now);
                builder.Property(x => x.RepairDate).IsRequired().HasColumnName("repair_date").HasColumnType("DATETIME");
                builder.Property(x => x.ChangeDateRegistrNumber).HasColumnName("change_date_registration").HasColumnType("DATETIME").HasDefaultValue(null);
                builder.Property(x => x.OrdinalNumber).IsRequired().HasColumnName("ordinal_number").HasColumnType("INTEGER");
                builder.Property(x => x.RegistrationNumber).IsRequired().HasColumnName("registration_numer").HasMaxLength(128);
                builder.Property(x => x.Note).HasColumnName("note").HasMaxLength(256);
                builder.HasMany(x => x.Documents).WithMany().UsingEntity(x => x.ToTable("execut_repair_documentation"));
            });
        }
    }
}
