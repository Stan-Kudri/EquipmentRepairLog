using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Extension;

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
                throw new EquipmentException("Error in KKS typing by user.");
            }
            else
            {
                foreach (var addItem in result)
                {
                    addKKSEquipments.Add(new KKSEquipment() { Equipment = kKSEquipment.Equipment, EquipmentType = kKSEquipment.EquipmentType, KKS = addItem });
                }
            }

            dbContext.AddMissingEquipmentDocuments(addKKSEquipments);
        }

        public void AddRangeEquipment(List<KKSEquipment> kKSEquipments)
        {
            ArgumentNullException.ThrowIfNull(kKSEquipments);

            var addKKSEquipments = new List<KKSEquipment>();

            foreach (var item in kKSEquipments)
            {
                if (!item.KKS.KKSValidation(out var result))
                {
                    throw new EquipmentException("Error in KKS typing by user.");
                }
                else
                {
                    foreach (var addItem in result)
                    {
                        addKKSEquipments.Add(new KKSEquipment() { Equipment = item.Equipment, EquipmentType = item.EquipmentType, KKS = addItem });
                    }
                }
            }

            dbContext.AddMissingEquipmentDocuments(addKKSEquipments);
        }
    }
}
