using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Extension;
using System.Data.Entity;

namespace EquipmentRepairLog.Core.Service
{
    public class EquipmentService(AppDbContext dbContext)
    {
        public void AddEquipment(KKSEquipment kKSEquipment)
        {
            ArgumentNullException.ThrowIfNull(kKSEquipment);

            var addKKSEquipments = new List<KKSEquipment>();

            if (!kKSEquipment.KKS.KKSValidation(out var result))
            {
                throw new EquipmentRepairLogException("Error in KKS typing by user.");
            }
            else
            {
                foreach (var addItem in result)
                {
                    addKKSEquipments.Add(new KKSEquipment() { Equipment = kKSEquipment.Equipment, EquipmentType = kKSEquipment.EquipmentType, KKS = addItem });
                }
            }

            AddMissingEquipmentDocuments(addKKSEquipments);
        }

        public void AddRangeEquipment(List<KKSEquipment> kKSEquipments)
        {
            ArgumentNullException.ThrowIfNull(kKSEquipments);

            var addKKSEquipments = new List<KKSEquipment>();

            foreach (var item in kKSEquipments)
            {
                if (!item.KKS.KKSValidation(out var result))
                {
                    throw new EquipmentRepairLogException("Error in KKS typing by user.");
                }
                else
                {
                    foreach (var addItem in result)
                    {
                        addKKSEquipments.Add(new KKSEquipment() { Equipment = item.Equipment, EquipmentType = item.EquipmentType, KKS = addItem });
                    }
                }
            }

            AddMissingEquipmentDocuments(addKKSEquipments);
        }

        private void AddMissingEquipmentDocuments(List<KKSEquipment> kksEquipments)
        {
            var transaction = dbContext.Database.BeginTransaction();

            try
            {
                //Создание списков типа/марки и вида оборудования
                var equipmentsType = kksEquipments.Select(e => e.EquipmentType.Name).Distinct().ToList();
                var equipments = kksEquipments.Select(e => e.Equipment.Name).Distinct().ToList();
                var listKKS = kksEquipments.Select(e => e.KKS).Distinct().ToList();

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
                    item.EquipmentTypeId = item.EquipmentType.Id;
                }

                var equipmentsDB = dbContext.KKSEquipments.AsNoTracking().Where(e => listKKS.Contains(e.KKS)).Select(e => new { e.EquipmentId, e.EquipmentTypeId, e.KKS }).ToList();
                var addNewKKSEquipments = kksEquipments.ExceptBy(equipmentsDB.Select(e => e.KKS), e => e.KKS).ToList();
                dbContext.KKSEquipments.AddRange(addNewKKSEquipments);

                foreach (var addItem in kksEquipments)
                {
                    for (int i = 0; i < equipmentsDB.Count; i++)
                    {
                        if (equipmentsDB[i].KKS == addItem.KKS
                                                && (equipmentsDB[i].EquipmentId != addItem.EquipmentId
                                                    || equipmentsDB[i].EquipmentTypeId != addItem.EquipmentTypeId))
                        {
                            dbContext.KKSEquipments.Add(addItem);
                        }
                    }
                }

                dbContext.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                throw new ArgumentException("Error in save data.");
            }


        }
    }
}
