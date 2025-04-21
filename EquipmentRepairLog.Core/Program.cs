using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;
using Microsoft.Extensions.DependencyInjection;

var db = new DbContextFactory().Create();

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

var docFirst = new Document()
{
    Division = division,
    DocumentType = docTypeFirst,
    OrdinalNumber = 1,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    RegistrationNumber = "FirstNumber"
};
var docSecond = new Document()
{
    Division = division,
    DocumentType = docTypeSecond,
    OrdinalNumber = 1,
    RepairFacility = repairFacility,
    RepairDate = DateTime.Now,
    RegistrationNumber = "SecondNumber"
};

docFirst.Documents.Add(docSecond);
docSecond.Documents.Add(docFirst);

kks.KKSEquipmentDocuments.AddRange(docFirst, docSecond);
perfomer.Documents.AddRange(docFirst, docSecond);

db.Documents.AddRange(docFirst, docSecond);
db.SaveChanges();

static IServiceCollection AppServiceDI()
            => new ServiceCollection().AddSingleton<AppDbContext>()
                                      .AddScoped(e => e.GetRequiredService<DbContextFactory>().Create());
