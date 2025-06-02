using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.Users;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.FactoryData;
using EquipmentRepairLog.Core.Service;
using Microsoft.Extensions.DependencyInjection;

using var db = await new DbContextFactory().CreateAsync();
var documentFactory = new DocumentFactroy(db);
var documentService = new DocumentService(db, documentFactory);

var divisionExe = new DivisionFactory().Create("Отдел под", "ОППР", 0);

var equipment = new Equipment() { Name = "Клапан запорный", Description = "Клапан новый" };
db.Equipments.Add(equipment);

var equipmentType = new EquipmentType() { Name = "КПЛВ.49833-12", Equipment = equipment, EquipmentId = equipment.Id };
db.EquipmentTypes.Add(equipmentType);

var kks = new KKSEquipment() { Equipment = equipment, EquipmentType = equipmentType, KKS = "10KAA22AA345", EquipmentId = equipment.Id, EquipmentTypeId = equipmentType.Id };
db.KKSEquipments.Add(kks);

var division = new Division() { Name = "Реакторный цех", Abbreviation = "РЦ", Number = 21 };
db.Divisions.Add(division);

var docTypeFirst = new DocumentType()
{
    Name = "Акт выполненных работ",
    IsOnlyTypeDocInERD = true,
    ExecutiveRepairDocNumber = 24,
    Abbreviation = "АВР",
};
db.DocumentTypes.Add(docTypeFirst);

var docTypeSecond = new DocumentType()
{
    Name = "Ведомость выполненных работ",
    IsOnlyTypeDocInERD = false,
    ExecutiveRepairDocNumber = 29,
    Abbreviation = "ВВР",
};
db.DocumentTypes.Add(docTypeSecond);

var perfomer = new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" };
db.Perfomers.Add(perfomer);

var repairFacility = new RepairFacility() { Number = 1, Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1" };
db.RepairFacilities.Add(repairFacility);

await db.SaveChangesAsync();

var docFirst = new DocumentModelCreator()
{
    Division = division,
    DocumentType = docTypeFirst,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    KKSEquipment = new List<KKSEquipment>() { kks },
    Perfomers = new List<Perfomer>() { perfomer },
    DivisionId = division.Id,
    DocumentTypeId = docTypeFirst.Id,
    RepairFacilityId = repairFacility.Id,
    RegistrationDate = DateTime.Now,
};
var docSecond = new DocumentModelCreator()
{
    Division = division,
    DocumentType = docTypeSecond,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    KKSEquipment = new List<KKSEquipment>() { kks },
    Perfomers = new List<Perfomer>() { perfomer },
    DivisionId = division.Id,
    DocumentTypeId = docTypeSecond.Id,
    RepairFacilityId = repairFacility.Id,
    RegistrationDate = DateTime.Now,
};

await documentService.AddAllDocumentsAsync(new List<DocumentModelCreator> { docFirst, docSecond });

var userService = new UserService(db, new UserValidator());

await userService.AddAsync("Stan228337", "Qav228337");

db.ChangeTracker.Clear();

var equipmentNewFirst = new Equipment() { Name = "Клапан запорный", Description = "Клапан новый" };

var equipmentTypeNewFirst = new EquipmentType() { Name = "НГ-2265", Equipment = equipmentNewFirst, EquipmentId = equipmentNewFirst.Id };

var kksNewFirst = new KKSEquipment()
{
    Equipment = equipmentNewFirst,
    EquipmentType = equipmentTypeNewFirst,
    KKS = "20KAA22AA345 -- 20KAA21AA345 20KAA22AA345",
    EquipmentId = equipmentNewFirst.Id,
    EquipmentTypeId = equipmentTypeNewFirst.Id,
};
var kksNewSecond = new KKSEquipment()
{
    Equipment = equipmentNewFirst,
    EquipmentType = equipmentTypeNewFirst,
    KKS = "20KAA11AA345 -- 20KAA21AA335 20KAA22AA325",
    EquipmentId = equipmentNewFirst.Id,
    EquipmentTypeId = equipmentTypeNewFirst.Id,
};

var docNewFirst = new DocumentModelCreator()
{
    Division = division,
    DocumentType = docTypeSecond,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    KKSEquipment = new List<KKSEquipment>() { kksNewFirst, kksNewSecond },
    Perfomers = new List<Perfomer>() { perfomer },
    DivisionId = division.Id,
    DocumentTypeId = docTypeSecond.Id,
    RepairFacilityId = repairFacility.Id,
    RegistrationDate = DateTime.Now,
};
var equipmentService = new EquipmentService(db);
await equipmentService.AddRangeEquipment([kksNewFirst, kksNewSecond]);

static IServiceCollection AppServiceDI()
            => new ServiceCollection().AddSingleton<AppDbContext>()
                                      .AddScoped(e => e.GetRequiredService<DbContextFactory>().CreateAsync());
