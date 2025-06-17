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
        public async Task AddEquipmentAsync(KKSEquipmentRequest kksEquipment)
        {
            BusinessLogicException.ThrowIfNull(kksEquipment);
            BusinessLogicException.ThrowIfNullOrEmpty(kksEquipment.Equipment);
            BusinessLogicException.ThrowIfNullOrEmpty(kksEquipment.EquipmentType);
            BusinessLogicException.ThrowIfNullOrEmpty(kksEquipment.KKS);

            var resultKKS = KKS.CreateArray(kksEquipment.KKS).ToList();

            if (resultKKS.Any(e => e.HasError))
            {
                throw new BusinessLogicException($"Error naming:{Environment.NewLine},{resultKKS.JoinErrorToString()}");
            }

            var addKKSEquipments = resultKKS.ConvertAll(addItem => new KKSEquipmentRequest
            {
                Equipment = kksEquipment.Equipment,
                EquipmentType = kksEquipment.EquipmentType,
                KKS = addItem.Value!.Value,
            });

            await AddEquipmentsCoreAsync(addKKSEquipments);
        }

        public async Task AddRangeEquipmentAsync(List<KKSEquipmentRequest> kksEquipments)
        {
            BusinessLogicException.ThrowIfNull(kksEquipments);

            foreach (var item in kksEquipments)
            {
                await AddEquipmentAsync(item);
            }
        }

        // Disable BCC2008
        private async Task AddEquipmentsCoreAsync(List<KKSEquipmentRequest> models, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
                    {
                        // Извлечение уникальных значений для оборудования, типов оборудования и KKS
                        var equipmentNames = models.Select(model => model.Equipment).ToHashSet();
                        var equipmentTypeNames = models.Select(model => model.EquipmentType).ToHashSet();
                        var kksList = models.Select(model => model.KKS).ToHashSet();

                        // Поиск и добавление в БД отсутствующих видов оборудовния
                        var existingEquipmentNames = dbContext.Equipments.AsNoTracking()
                                                                         .Where(e => equipmentNames.Contains(e.Name))
                                                                         .Select(e => e.Name)
                                                                         .ToHashSet();
                        var missingEquipments = models.Where(kks => !existingEquipmentNames.Contains(kks.Equipment))
                                                                  .Select(kks => kks.Equipment)
                                                                  .ToHashSet();
                        if (missingEquipments.Count != 0)
                        {
                            var equipments = missingEquipments.Select(equipment => new Equipment() { Name = equipment })
                                                              .ToList();

                            await dbContext.Equipments.AddRangeAsync(equipments, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }

                        // Поиск и добавление в БД отсутствующих типов/марок оборудовния
                        var containsDBEquipmentType = dbContext.EquipmentTypes.AsNoTracking()
                                                                              .Where(equipmentType => equipmentTypeNames.Contains(equipmentType.Name))
                                                                              .Select(equipmentType => equipmentType.Name)
                                                                              .ToHashSet();
                        var missingEquipmentType = models.Where(kksModel => !containsDBEquipmentType.Contains(kksModel.EquipmentType))
                                                         .ToHashSet();

                        if (missingEquipmentType.Count != 0)
                        {
                            var equipmentFromDBByName = dbContext.Equipments.Where(equipment => equipmentNames.Contains(equipment.Name))
                                                                            .ToList();

                            // Создание списка с полными данными объекта для добавления
                            var newEquipmentTypes = missingEquipmentType.Join(equipmentFromDBByName,
                                                                              missingItem => missingItem.Equipment,
                                                                              dbItem => dbItem.Name,
                                                                              (missingItem, dbEquipment) => new EquipmentType()
                                                                              {
                                                                                  Name = missingItem.EquipmentType,
                                                                                  EquipmentId = dbEquipment.Id,
                                                                                  Equipment = dbEquipment,
                                                                              }).ToHashSet();

                            await dbContext.EquipmentTypes.AddRangeAsync(newEquipmentTypes, cancellationToken);
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }

                        var equipmentsDictionary = dbContext.Equipments.AsNoTracking()
                                                                       .Where(eqipment => equipmentNames.Contains(eqipment.Name))
                                                                       .ToDictionary(e => e.Name);

                        var equipmentsTypeDictionary = dbContext.EquipmentTypes.AsNoTracking()
                                                                               .Where(equipmentType => equipmentTypeNames.Contains(equipmentType.Name))
                                                                               .ToDictionary(e => e.Name);

                        // Обновление объектов в kksEquipmentsModel
                        foreach (var item in models)
                        {
                            if (equipmentsDictionary.TryGetValue(item.Equipment, out var equipment))
                            {
                                item.EquipmentId = equipment.Id;
                            }

                            if (equipmentsTypeDictionary.TryGetValue(item.EquipmentType, out var equipmentType))
                            {
                                item.EquipmentTypeId = equipmentType.Id;
                            }
                        }

                        // Добавление данных для объектов
                        var kksEquipments = models.Select(item => new KKSEquipment()
                        {
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
