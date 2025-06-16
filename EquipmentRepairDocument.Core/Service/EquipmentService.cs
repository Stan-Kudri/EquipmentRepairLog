using System.Data.Entity;
using EquipmentRepairDocument.Core.Data.EquipmentModel;
using EquipmentRepairDocument.Core.Data.ValidationData;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Extension;

namespace EquipmentRepairDocument.Core.Service
{
    public class EquipmentService(AppDbContext dbContext)
    {
        public async Task AddEquipmentAsync(KKSEquipmentModel kksEquipment)
        {
            BusinessLogicException.ThrowIfNull(kksEquipment);
            BusinessLogicException.ThrowIfNull(kksEquipment.Equipment);
            BusinessLogicException.ThrowIfNull(kksEquipment.EquipmentType);
            BusinessLogicException.ThrowIfNullOrEmpty(kksEquipment.KKS);

            var resultKKS = KKS.CreateArray(kksEquipment.KKS).ToList();

            if (resultKKS.Any(e => e.HasError))
            {
                throw new BusinessLogicException($"Error naming:{Environment.NewLine},{resultKKS.JoinErrorToString()}");
            }

            var addKKSEquipments = resultKKS.ConvertAll(addItem => new KKSEquipmentModel
            {
                Equipment = kksEquipment.Equipment,
                EquipmentType = kksEquipment.EquipmentType,
                KKS = addItem.Value!.Value,
            });

            await AddMissingEquipmentDocuments(addKKSEquipments);
        }

        public async Task AddRangeEquipmentAsync(List<KKSEquipmentModel> kksEquipments)
        {
            BusinessLogicException.ThrowIfNull(kksEquipments);

            foreach (var item in kksEquipments)
            {
                await AddEquipmentAsync(item);
            }
        }

        // Disable BCC2008
        private async Task AddMissingEquipmentDocuments(List<KKSEquipmentModel> models, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
                    {
                        // Извлечение уникальных значений для оборудования, типов оборудования и KKS
                        var equipmentNames = models.Select(model => model.Equipment.Name).Distinct().ToList();
                        var equipmentTypeNames = models.Select(model => model.EquipmentType.Name).Distinct().ToList();
                        var kksList = models.Select(model => model.KKS).Distinct().ToList();

                        // Поиск и добавление в БД отсутствующих видов оборудовния
                        var existingEquipmentNames = dbContext.Equipments.AsNoTracking()
                                                                         .Where(e => equipmentNames.Contains(e.Name))
                                                                         .Select(e => e.Name)
                                                                         .ToHashSet();
                        var missingEquipments = models.Where(kks => !existingEquipmentNames.Contains(kks.Equipment.Name))
                                                                  .Select(kks => kks.Equipment)
                                                                  .Distinct()
                                                                  .ToList();
                        if (missingEquipments.Count != 0)
                        {
                            await dbContext.Equipments.AddRangeAsync(missingEquipments, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }

                        // Поиск и добавление в БД отсутствующих типов/марок оборудовния
                        var containsDBEquipmentType = dbContext.EquipmentTypes.AsNoTracking()
                                                                              .Where(equipmentType => equipmentTypeNames.Contains(equipmentType.Name))
                                                                              .Select(equipmentType => equipmentType.Name)
                                                                              .ToHashSet();
                        var missingEquipmentType = models.Where(kksModel => !containsDBEquipmentType.Contains(kksModel.EquipmentType.Name))
                                                                     .Select(kksModel => kksModel.EquipmentType)
                                                                     .Distinct()
                                                                     .ToList();
                        if (missingEquipmentType.Count != 0)
                        {
                            var equipmentHashSet = new HashSet<string>(equipmentNames);
                            var equipmentFromDBByName = dbContext.Equipments.Where(equipment => equipmentNames.Contains(equipment.Name))
                                                                            .ToList();

                            // Создание списка с полными данными объекта для добавления
                            var newEquipmentTypes = missingEquipmentType.Join(equipmentFromDBByName,
                                                                              missingItem => missingItem.Equipment?.Name,
                                                                              dbItem => dbItem.Name,
                                                                              (missingItem, dbEquipment) => new EquipmentType()
                                                                              {
                                                                                  Name = missingItem.Name,
                                                                                  EquipmentId = dbEquipment.Id,
                                                                                  Equipment = dbEquipment,
                                                                              });
                            await dbContext.EquipmentTypes.AddRangeAsync(newEquipmentTypes, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }

                        var equipmentsDictionary = dbContext.Equipments.AsNoTracking()
                                                                       .Where(eqipment => equipmentNames.Contains(eqipment.Name))
                                                                       .GroupBy(eqipment => eqipment.Name)
                                                                       .ToDictionary(e => e.Key, e => e.First());

                        var equipmentsTypeDictionary = dbContext.EquipmentTypes.AsNoTracking()
                                                                               .Where(equipmentType => equipmentTypeNames.Contains(equipmentType.Name))
                                                                               .GroupBy(equipmentType => equipmentType.Name)
                                                                               .ToDictionary(e => e.Key, e => e.First());

                        // Обновление объектов в kksEquipmentsModel
                        foreach (var item in models)
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
                        var kksEquipments = models.Select(item => new KKSEquipment()
                        {
                            Equipment = item.Equipment,
                            EquipmentType = item.EquipmentType,
                            KKS = item.KKS,
                            EquipmentId = item.EquipmentId,
                            EquipmentTypeId = item.EquipmentTypeId,
                        });

                        // Получение существующих KKS из БД
                        var existingKKS = dbContext.KKSEquipments.AsNoTracking()
                                                                 .Where(kks => kksList.Contains(kks.KKS))
                                                                 .Select(kks => new { kks.EquipmentId, kks.EquipmentTypeId, kks.KKS })
                                                                 .ToHashSet();

                        // Получение отсутствующих KKS в БД
                        var missingKKSEquipments = kksEquipments.Where(kks => !existingKKS.Any(db => db.KKS == kks.KKS)).ToList();

                        // Добавление новых KKS со всеми данными в БД
                        await dbContext.KKSEquipments.AddRangeAsync(missingKKSEquipments, cancellationToken);

                        // Фильтрация записей, если отсутствует KKS или данные отличаются – добавляем новые записи
                        var missingKKS = kksEquipments.Where(kks => existingKKS.Any(db => db.KKS == kks.KKS &&
                                                                                          (db.EquipmentId != kks.EquipmentId ||
                                                                                           db.EquipmentTypeId != kks.EquipmentTypeId))).ToList();

                        await dbContext.KKSEquipments.AddRangeAsync(missingKKS, cancellationToken);
                        await dbContext.SaveChangesAsync(cancellationToken);
                        return DBNull.Value;
                    },
                    cancellationToken);
        }
    }
}
