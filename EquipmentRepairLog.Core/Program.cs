using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Data.User;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Service;
using Microsoft.Extensions.DependencyInjection;

var db = new DbContextFactory().Create();
var documentService = new DocumentService(db);

var equipment = new Equipment() { Name = "Клапан запорный", Description = "Клапан новый" };
db.Equipments.Add(equipment);

var equipmentType = new EquipmentType() { Name = "КПЛВ.49833-12", Equipment = equipment };
db.EquipmentTypes.Add(equipmentType);


var kks = new KKSEquipment() { Equipment = equipment, EquipmentType = equipmentType, KKS = "10KAA22AA345" };
db.KKSEquipments.Add(kks);

var division = new Division() { Name = "Реакторный цех", Abbreviation = "РЦ", Number = 21 };
db.Divisions.Add(division);

var docTypeFirst = new DocumentType()
{
    Name = "Акт выполненных работ",
    IsOnlyTypeDocInRepairLog = true,
    ExecutiveRepairDocNumber = 24,
    Abbreviation = "АВР"
};
db.DocumentTypes.Add(docTypeFirst);

var docTypeSecond = new DocumentType()
{
    Name = "Ведомость выполненных работ",
    IsOnlyTypeDocInRepairLog = false,
    ExecutiveRepairDocNumber = 29,
    Abbreviation = "ВВР"
};
db.DocumentTypes.Add(docTypeSecond);

var perfomer = new Perfomer() { Name = "Атомтехэнерго", Abbreviation = "АТЭ" };
db.Perfomers.Add(perfomer);

var repairFacility = new RepairFacility() { Number = 1, Name = "Энергоблок № 1", Abbreviation = "ЭБ № 1" };
db.RepairFacilities.Add(repairFacility);

db.SaveChanges();

var docFirst = new Document()
{
    Division = division,
    DocumentType = docTypeFirst,
    OrdinalNumber = 1,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    RegistrationNumber = "FirstNumber",
    KKSEquipment = new List<KKSEquipment>() { kks },
    Perfomers = new List<Perfomer>() { perfomer }
};
var docSecond = new Document()
{
    Division = division,
    DocumentType = docTypeSecond,
    OrdinalNumber = 1,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    RegistrationNumber = "SecondNumber",
    KKSEquipment = new List<KKSEquipment>() { kks },
    Perfomers = new List<Perfomer>() { perfomer }
};

documentService.AddAllDocuments(new List<Document> { docFirst, docSecond });

var user = new User("Stan228337", "Qav228337");
db.Users.Add(user);
db.SaveChanges();

db.ChangeTracker.Clear();
documentService.RemoveERD(docFirst.RegistrationNumber);

static IServiceCollection AppServiceDI()
            => new ServiceCollection().AddSingleton<AppDbContext>()
                                      .AddScoped(e => e.GetRequiredService<DbContextFactory>().Create());
