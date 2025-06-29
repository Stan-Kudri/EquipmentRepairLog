using EquipmentRepairDocument.Core.Data.DocumentModel;
using EquipmentRepairDocument.Core.Data.EquipmentModel;
using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.Data.ValidationData;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.FactoryData;
using EquipmentRepairDocument.Core.Service;
using EquipmentRepairDocument.Core.Service.Users;

using var db = await new DbContextFactory().CreateAsync();
var equipmentService = new EquipmentService(db);
var documentFactory = new DocumentFactory(db, equipmentService);
var documentService = new DocumentService(db, documentFactory);

var divisionExe = new DivisionFactory().Create("Отдел под", "ОППР", 0);

var equipment = new Equipment() { Name = "Клапан запорный", Description = "Клапан новый" };
db.Equipments.Add(equipment);

var equipmentType = new EquipmentType() { Name = "КПЛВ.49833-12", Equipment = equipment, EquipmentId = equipment.Id };
db.EquipmentTypes.Add(equipmentType);

var kks = new KKSEquipmentRequest() { Equipment = "КПЛВ.49833-12", EquipmentType = "Клапан запорный", KKS = "10KAA22AA345" };

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

var docFirst = new DocumentCreateRequest()
{
    Division = division,
    DocumentType = docTypeFirst,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    KKSEquipment = new List<KKSEquipmentRequest>() { kks },
    Perfomers = new List<Perfomer>() { perfomer },
    DivisionId = division.Id,
    DocumentTypeId = docTypeFirst.Id,
    RepairFacilityId = repairFacility.Id,
    RegistrationDate = DateTime.Now,
};
var docSecond = new DocumentCreateRequest()
{
    Division = division,
    DocumentType = docTypeSecond,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    KKSEquipment = new List<KKSEquipmentRequest>() { kks },
    Perfomers = new List<Perfomer>() { perfomer },
    DivisionId = division.Id,
    DocumentTypeId = docTypeSecond.Id,
    RepairFacilityId = repairFacility.Id,
    RegistrationDate = DateTime.Now,
};

await documentService.AddAllDocumentsAsync(new List<DocumentCreateRequest> { docFirst, docSecond });

var userService = new UserService(db, new UserValidator(), new BCryptPasswordHasher());

await userService.AddAsync("Stan228337", "Qav228337");

db.ChangeTracker.Clear();

var equipmentNewFirst = "Клапан запорный";

var equipmentTypeNewFirst = "НГ-2265";

var kksNewFirst = new KKSEquipmentRequest()
{
    Equipment = equipmentNewFirst,
    EquipmentType = equipmentTypeNewFirst,
    KKS = "20KAA22AA345 -- 20KAA21AA345 20KAA22AA345",
};
var kksNewSecond = new KKSEquipmentRequest()
{
    Equipment = equipmentNewFirst,
    EquipmentType = equipmentTypeNewFirst,
    KKS = "20KAA11AA345 -- 20KAA21AA335 20KAA22AA325",
};

await equipmentService.AddRangeEquipmentAsync([kksNewFirst, kksNewSecond]);
