using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.ValidationData;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using EquipmentRepairLog.Core.Extension;
using System.Data.Entity;

namespace EquipmentRepairLog.Core.Service
{
    public class EquipmentService(AppDbContext dbContext)
    {
        public void AddEquipment(KKSEquipment kKSEquipment)
        {
            ArgumentNullException.ThrowIfNull(kKSEquipment);
            ArgumentNullException.ThrowIfNull(kKSEquipment.Equipment);
            ArgumentNullException.ThrowIfNull(kKSEquipment.EquipmentType);

            var addKKSEquipments = new List<KKSEquipmentModel>();

            if (!kKSEquipment.KKS.KKSValidation(out var result))
            {
                throw new EquipmentRepairLogException($"Error in kks naming {result[0]}.");
            }
            else
            {
                foreach (var addItem in result)
                {
                    addKKSEquipments.Add(new KKSEquipmentModel() { Equipment = kKSEquipment.Equipment, EquipmentType = kKSEquipment.EquipmentType, KKS = addItem });
                }
            }

            AddMissingEquipmentDocuments(addKKSEquipments);
        }

        public void AddRangeEquipment(List<KKSEquipment> kKSEquipments)
        {
            ArgumentNullException.ThrowIfNull(kKSEquipments);

            var addKKSEquipments = new List<KKSEquipmentModel>();

            foreach (var item in kKSEquipments)
            {
                ArgumentNullException.ThrowIfNull(item.Equipment);
                ArgumentNullException.ThrowIfNull(item.EquipmentType);

                if (!item.KKS.KKSValidation(out var result))
                {
                    throw new EquipmentRepairLogException($"Error in kks naming {result[0]}.");
                }
                else
                {
                    foreach (var addItem in result)
                    {
                        addKKSEquipments.Add(new KKSEquipmentModel() { Equipment = item.Equipment, EquipmentType = item.EquipmentType, KKS = addItem });
                    }
                }
            }

            AddMissingEquipmentDocuments(addKKSEquipments);
        }

        private void AddMissingEquipmentDocuments(List<KKSEquipmentModel> kksEquipmentsModel)
        {
            var transaction = dbContext.Database.BeginTransaction();

            try
            {
                //Создание списков типа/марки и вида оборудования
                var equipmentsType = kksEquipmentsModel.Select(e => e.EquipmentType.Name).Distinct().ToList();
                var equipments = kksEquipmentsModel.Select(e => e.Equipment.Name).Distinct().ToList();
                var listKKS = kksEquipmentsModel.Select(e => e.KKS).Distinct().ToList();

                //Поиск и добавление отсутствующих видов оборудовния
                var containsDBEquipment = dbContext.Equipments.AsNoTracking().Where(e => equipments.Contains(e.Name)).Select(e => e.Name).ToList();
                var addEquipment = kksEquipmentsModel.Where(e => !containsDBEquipment.Contains(e.Equipment.Name)).Select(e => e.Equipment);
                if (addEquipment.Any())
                {
                    dbContext.Equipments.AddRange(addEquipment);
                    dbContext.SaveChanges();
                }

                //Поиск и добавление отсутствующих типов/марок оборудовния
                var containsDBEquipmentType = dbContext.EquipmentTypes.AsNoTracking().Where(e => equipmentsType.Contains(e.Name)).Select(e => e.Name).ToList();
                var missingEquipmentType = kksEquipmentsModel.Where(e => !containsDBEquipmentType.Contains(e.EquipmentType.Name)).Select(e => e.EquipmentType);
                if (missingEquipmentType.Any())
                {
                    var addEquipmentType = missingEquipmentType.Join(dbContext.Equipments,
                                                                                          a => a.Equipment?.Name,
                                                                                          b => b.Name,
                                                                                          (a, b) => new EquipmentType() { Name = a.Name, EquipmentId = b.Id, Equipment = b });
                    dbContext.EquipmentTypes.AddRange(addEquipmentType);
                    dbContext.SaveChanges();
                }

                //Добавление полей Id в экземпляр класса Document.
                var equipmentsTypeId = dbContext.EquipmentTypes.AsNoTracking().Where(e => equipmentsType.Contains(e.Name));
                var equipmentsId = dbContext.Equipments.AsNoTracking().Where(e => equipments.Contains(e.Name));

                //Добавление Id в поля класса.
                foreach (var item in kksEquipmentsModel)
                {
                    item.Equipment = equipmentsId.First(e => e.Name == item.Equipment.Name);
                    item.EquipmentId = item.Equipment.Id;
                    item.EquipmentType = equipmentsTypeId.First(e => e.Name == item.EquipmentType.Name);
                    item.EquipmentTypeId = item.EquipmentType.Id;
                }

                //Поиск дублирующих KKS из переданных и БД
                var equipmentsDB = dbContext.KKSEquipments.AsNoTracking().Where(e => listKKS.Contains(e.KKS)).Select(e => new { e.EquipmentId, e.EquipmentTypeId, e.KKS }).ToList();

                var kksEquipments = kksEquipmentsModel.Select(item => new KKSEquipment()
                {
                    Equipment = item.Equipment,
                    EquipmentType = item.EquipmentType,
                    KKS = item.KKS,
                    EquipmentId = item.EquipmentId,
                    EquipmentTypeId = item.EquipmentTypeId
                });

                var addNewKKSEquipments = kksEquipments.ExceptBy(equipmentsDB.Select(e => e.KKS), e => e.KKS).ToList();

                //Добавление новых KKS со всеми данными в БД
                dbContext.KKSEquipments.AddRange(addNewKKSEquipments);

                //Поиск отличий полей от дублирующих KKS и добавление их в БД, если есть отличия 
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
                transaction.Rollback();
                throw new TransactionAppException("Error in save data.");
            }
        }
    }
}
