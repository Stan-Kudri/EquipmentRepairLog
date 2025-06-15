using System.Data.Entity;
using EquipmentRepairDocument.Core.Data.EquipmentModel;
using EquipmentRepairDocument.Core.Data.ValidationData;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Exceptions.AppException;
using EquipmentRepairDocument.Core.Extension;

namespace EquipmentRepairDocument.Core.Service
{
    public class EquipmentService(AppDbContext dbContext)
    {
        public async Task AddEquipmentAsync(KKSEquipment kksEquipment)
        {
            BusinessLogicException.ThrowIfNull(kksEquipment);
            BusinessLogicException.ThrowIfNull(kksEquipment.Equipment);
            BusinessLogicException.ThrowIfNull(kksEquipment.EquipmentType);
            BusinessLogicException.ThrowIfNullOrEmpty(kksEquipment.KKS);

            var resultKKS = KKS.CreateArray(kksEquipment.KKS)
                               .Where(e => !e.HasError)
                               .Select(e => e.Value)
                               .ToList() ?? throw new NotFoundException("No KKS elements found to add.");

            if (resultKKS.Count == 0)
            {
                throw new BusinessLogicException($"Error naming:{Environment.NewLine}{KKS.CreateArray(kksEquipment.KKS).JoinErrorToString()}");
            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var addKKSEquipments = resultKKS.Select(addItem => new KKSEquipmentModel
            {
                Equipment = kksEquipment.Equipment,
                EquipmentType = kksEquipment.EquipmentType,
                KKS = addItem.Value,
            }).ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            await AddMissingEquipmentDocuments(addKKSEquipments);
        }

        public async Task AddRangeEquipmentAsync(List<KKSEquipment> kksEquipments)
        {
            BusinessLogicException.ThrowIfNull(kksEquipments);

            foreach (var item in kksEquipments)
            {
                await AddEquipmentAsync(item);
            }
        }

        // Disable BCC2008
        private async Task AddMissingEquipmentDocuments(List<KKSEquipmentModel> kksEquipmentsModel, CancellationToken cancellationToken = default)
            => await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание переданного списка оборудования
                var equipments = kksEquipmentsModel.Select(e => e.Equipment.Name).Distinct().ToList();

                // Создание переданного списка типа оборудования
                var equipmentsType = kksEquipmentsModel.Select(e => e.EquipmentType.Name).Distinct().ToList();

                // Создание переданного списка KKS
                var listKKS = kksEquipmentsModel.Select(e => e.KKS).Distinct().ToList();

                // Поиск и добавление в БД отсутствующих видов оборудовния
                var containsDBEquipment = dbContext.Equipments.AsNoTracking().Where(e => equipments.Contains(e.Name)).Select(e => e.Name).ToList();
                var addEquipment = kksEquipmentsModel.Where(e => !containsDBEquipment.Contains(e.Equipment.Name)).Select(e => e.Equipment).Distinct().ToList();
                if (addEquipment.Count != 0)
                {
                    await dbContext.Equipments.AddRangeAsync(addEquipment, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                // Поиск и добавление в БД отсутствующих типов/марок оборудовния
                var containsDBEquipmentType = dbContext.EquipmentTypes.AsNoTracking().Where(e => equipmentsType.Contains(e.Name)).Select(e => e.Name).ToList();
                var missingEquipmentType = kksEquipmentsModel.Where(e => !containsDBEquipmentType.Contains(e.EquipmentType.Name)).Select(e => e.EquipmentType).Distinct().ToList();
                if (missingEquipmentType.Count != 0)
                {
                    var equipmentNames = new HashSet<string>(kksEquipmentsModel.Select(e => e.Equipment.Name).Distinct());
                    var equipmentByName = dbContext.Equipments.Where(e => equipmentNames.Contains(e.Name)).ToList();

                    // Создание списка с полными данными объекта для добавления
                    var addEquipmentType = missingEquipmentType.Join(equipmentByName,
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

                // Добавление данных для объектов
                var kksEquipments = kksEquipmentsModel.Select(item => new KKSEquipment()
                {
                    Equipment = item.Equipment,
                    EquipmentType = item.EquipmentType,
                    KKS = item.KKS,
                    EquipmentId = item.EquipmentId,
                    EquipmentTypeId = item.EquipmentTypeId,
                });

                // Поиск дубликатов KKS в БД
                var equipmentsDB = dbContext.KKSEquipments.AsNoTracking().Where(e => listKKS.Contains(e.KKS)).Select(e => new { e.EquipmentId, e.EquipmentTypeId, e.KKS }).ToList();

                // Поиск отсутствующих KKS в БД
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
