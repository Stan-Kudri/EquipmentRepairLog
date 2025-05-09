using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.DBContext;

namespace EquipmentRepairLog.Core.Extension
{
    public static class DocumentsDBExtension
    {
        public static bool DocumentsValidationDataDocumentTypeAndNumber(this AppDbContext dbContext, List<Document> documents)
            => documents.All(dbContext.DocValidDataDocumentTypeAndNumber);

        public static bool DocValidDataDocumentTypeAndNumber(this AppDbContext dbContext, Document document)
            => dbContext.Documents.FirstOrDefault(e => (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                           || e.RegistrationNumber == document.RegistrationNumber) == null;

        public static void AddMissingEquipmentDocuments(this AppDbContext dbContext, List<KKSEquipment> kksEquipments)
        {
            /*
            var transaction = dbContext.Database.BeginTransaction();
            try
            {
                transaction.Commit();
            }
            catch
            {
                throw new ArgumentException("Error in data.");
            }
            */

            //Создание списков типа/марки и вида оборудования
            var equipmentsType = kksEquipments.Select(e => e.EquipmentType.Name);
            var equipments = kksEquipments.Select(e => e.Equipment.Name);
            var listKKS = kksEquipments.Select(e => e.KKS);

            //Поиск и добавление отсутствующих видов оборудовния
            var containsDBEquipment = dbContext.Equipments.Where(e => equipments.Contains(e.Name)).Select(e => e.Name).ToList();
            var addEquipment = kksEquipments.Where(e => !containsDBEquipment.Contains(e.Equipment.Name)).Select(e => e.Equipment);
            if (addEquipment.Any())
            {
                dbContext.Equipments.AddRange(addEquipment);
                dbContext.SaveChanges();
            }

            //Поиск и добавление отсутствующих типов/марок оборудовния
            var containsDBEquipmentType = dbContext.EquipmentTypes.Where(e => equipmentsType.Contains(e.Name)).Select(e => e.Name).ToList();
            var missingEquipmentType = kksEquipments.Where(e => !containsDBEquipmentType.Contains(e.EquipmentType.Name)).Select(e => e.EquipmentType);
            if (missingEquipmentType.Any())
            {
                var addEquipmentType = missingEquipmentType.Join(dbContext.Equipments,
                                                                                      a => a.Equipment.Name,
                                                                                      b => b.Name,
                                                                                      (a, b) => new EquipmentType() { Name = a.Name, Equipment = b });
                dbContext.EquipmentTypes.AddRange(addEquipmentType);
                dbContext.SaveChanges();
            }

            //Добавление полей Id в экземпляр класса Document.
            var equipmentsTypeId = dbContext.EquipmentTypes.Where(e => equipmentsType.Contains(e.Name)).Select(e => new { EquipmentType = e }).ToList();
            var equipmentsId = dbContext.Equipments.Where(e => equipments.Contains(e.Name)).Select(e => new { Equipment = e }).ToList();

            //Добавление Id в поля класса.
            foreach (var item in kksEquipments)
            {
                item.Equipment = equipmentsId.First(e => e.Equipment.Name == item.Equipment.Name).Equipment;
                item.EquipmentId = item.Equipment.Id;
                item.EquipmentType = equipmentsTypeId.First(e => e.EquipmentType.Name == item.EquipmentType.Name).EquipmentType;
                item.EquipmentTypeId = item.Equipment.Id;
            }

            var equipmentsDB = dbContext.KKSEquipments.ToList();
            var kksDb = equipmentsDB.Select(e => e.KKS).ToList();

            foreach (var addItem in kksEquipments)
            {
                if (!kksDb.Contains(addItem.KKS))
                {
                    dbContext.KKSEquipments.Add(addItem);
                }
                else if (kksDb.Contains(addItem.KKS))
                {
                    for (int i = 0; i < equipmentsDB.Count(); i++)
                    {
                        if (equipmentsDB[i].KKS == addItem.KKS
                                                && (equipmentsDB[i].EquipmentId == addItem.EquipmentId
                                                    || equipmentsDB[i].EquipmentTypeId == addItem.EquipmentTypeId))
                        {
                            dbContext.KKSEquipments.Add(addItem);
                        }
                    }
                }
            }

            dbContext.SaveChanges();
        }
    }
}
