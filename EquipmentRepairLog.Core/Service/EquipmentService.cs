using System.Data.Entity;
using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.ValidationData;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Extension;

namespace EquipmentRepairLog.Core.Service
{
    public class EquipmentService(AppDbContext dbContext)
    {
        public async Task AddEquipment(KKSEquipment kksEquipment)
        {
            BusinessLogicException.ThrowIfNull(kksEquipment);
            BusinessLogicException.ThrowIfNull(kksEquipment.Equipment);
            BusinessLogicException.ThrowIfNull(kksEquipment.EquipmentType);
            BusinessLogicException.ThrowIfNullOrEmpty(kksEquipment.KKS);

            var addKKSEquipments = new List<KKSEquipmentModel>();
            var resultKKS = KKS.CreateArray(kksEquipment.KKS).ToList();

            if (resultKKS.Any(e => e.HasError))
            {
                throw new BusinessLogicException($"Error naming:{Environment.NewLine},{resultKKS.ErrorListMassage()}");
            }

            foreach (var addItem in resultKKS.Where(e => e.HasError == false).Select(e => e.Value))
            {
                if (addItem == null)
                {
                    throw new BusinessLogicException("KKS conversion error.");
                }
                else
                {
                    addKKSEquipments.Add(new KKSEquipmentModel() { Equipment = kksEquipment.Equipment, EquipmentType = kksEquipment.EquipmentType, KKS = addItem.Value });
                }
            }

            await AddMissingEquipmentDocuments(addKKSEquipments);
        }

        public async Task AddRangeEquipment(List<KKSEquipment> kksEquipments)
        {
            ArgumentNullException.ThrowIfNull(kksEquipments);

            var addKKSEquipments = new List<KKSEquipmentModel>();

            foreach (var item in kksEquipments)
            {
                BusinessLogicException.ThrowIfNull(item.Equipment);
                BusinessLogicException.ThrowIfNull(item.EquipmentType);
                BusinessLogicException.ThrowIfNullOrEmpty(item.KKS);

                var resultKKS = KKS.CreateArray(item.KKS).ToList();

                if (resultKKS.Any(e => e.HasError))
                {
                    throw new BusinessLogicException($"Error naming:{Environment.NewLine},{resultKKS.ErrorListMassage()}");
                }

                foreach (var addItem in resultKKS.Where(e => e.HasError == false).Select(e => e.Value))
                {
                    if (addItem == null)
                    {
                        throw new BusinessLogicException("KKS conversion error.");
                    }
                    else
                    {
                        addKKSEquipments.Add(new KKSEquipmentModel() { Equipment = item.Equipment, EquipmentType = item.EquipmentType, KKS = addItem.Value });
                    }
                }
            }

            await AddMissingEquipmentDocuments(addKKSEquipments);
        }

        // Disable BCC2008
        private async Task AddMissingEquipmentDocuments(List<KKSEquipmentModel> kksEquipmentsModel, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание списков типа/марки и вида оборудования
                var equipmentsType = kksEquipmentsModel.Select(e => e.EquipmentType.Name).Distinct().ToList();
                var equipments = kksEquipmentsModel.Select(e => e.Equipment.Name).Distinct().ToList();
                var listKKS = kksEquipmentsModel.Select(e => e.KKS).Distinct().ToList();

                // Поиск и добавление отсутствующих видов оборудовния
                var containsDBEquipment = dbContext.Equipments.AsNoTracking().Where(e => equipments.Contains(e.Name)).Select(e => e.Name).ToList();
                var addEquipment = kksEquipmentsModel.Where(e => !containsDBEquipment.Contains(e.Equipment.Name)).Select(e => e.Equipment).Distinct();
                if (addEquipment.Any())
                {
                    await dbContext.Equipments.AddRangeAsync(addEquipment, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                // Поиск и добавление отсутствующих типов/марок оборудовния
                var containsDBEquipmentType = dbContext.EquipmentTypes.AsNoTracking().Where(e => equipmentsType.Contains(e.Name)).Select(e => e.Name).ToList();
                var missingEquipmentType = kksEquipmentsModel.Where(e => !containsDBEquipmentType.Contains(e.EquipmentType.Name)).Select(e => e.EquipmentType).Distinct();
                if (missingEquipmentType.Any())
                {
                    var addEquipmentType = missingEquipmentType.Join(dbContext.Equipments,
                                                                      a => a.Equipment?.Name,
                                                                      b => b.Name,
                                                                      (a, b) => new EquipmentType()
                                                                      {
                                                                          Name = a.Name,
                                                                          EquipmentId = b.Id,
                                                                          Equipment = b,
                                                                      });
                    await dbContext.EquipmentTypes.AddRangeAsync(addEquipmentType, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                // Создание словоря для EquipmentTypes и Equipments.
                var equipmentsDictionary = dbContext.Equipments.AsNoTracking().Where(e => equipments.Contains(e.Name)).Select(e => e).ToList().DistinctBy(type => type.Name).ToDictionary(e => e.Name);
                var equipmentsTypeDictionary = dbContext.EquipmentTypes.AsNoTracking().Where(type => equipmentsType.Contains(type.Name)).ToList().DistinctBy(type => type.Name).ToDictionary(e => e.Name);

                // Добавление Id и объектов (Equipment/EquipmentType) в поля класса.
                foreach (var item in kksEquipmentsModel)
                {
                    if (equipmentsDictionary.TryGetValue(item.Equipment.Name, out var equipment))
                    {
                        item.Equipment = equipment;
                        item.EquipmentId = equipment.Id;
                    }

                    if (equipmentsTypeDictionary.TryGetValue(item.EquipmentType.Name, out var equipmentType))
                    {
                        item.EquipmentType = equipmentType;
                        item.EquipmentTypeId = equipmentType.Id;
                    }
                }

                // Поиск дублирующих KKS из переданных и БД
                var equipmentsDB = dbContext.KKSEquipments.AsNoTracking().Where(e => listKKS.Contains(e.KKS)).Select(e => new { e.EquipmentId, e.EquipmentTypeId, e.KKS }).ToList();

                var kksEquipments = kksEquipmentsModel.Select(item => new KKSEquipment()
                {
                    Equipment = item.Equipment,
                    EquipmentType = item.EquipmentType,
                    KKS = item.KKS,
                    EquipmentId = item.EquipmentId,
                    EquipmentTypeId = item.EquipmentTypeId,
                });

                var addNewKKSEquipments = kksEquipments.ExceptBy(equipmentsDB.Select(e => e.KKS), e => e.KKS).ToList();

                // Добавление новых KKS со всеми данными в БД
                await dbContext.KKSEquipments.AddRangeAsync(addNewKKSEquipments, cancellationToken);

                // Поиск отличий полей от дублирующих KKS и добавление их в БД, если есть отличия 
                foreach (var addItem in kksEquipments)
                {
                    for (var i = 0; i < equipmentsDB.Count; i++)
                    {
                        if (equipmentsDB[i].KKS == addItem.KKS
                            && (equipmentsDB[i].EquipmentId != addItem.EquipmentId
                            || equipmentsDB[i].EquipmentTypeId != addItem.EquipmentTypeId))
                        {
                            await dbContext.KKSEquipments.AddAsync(addItem, cancellationToken);
                        }
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }
    }
}
