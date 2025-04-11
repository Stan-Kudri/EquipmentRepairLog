using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class EquipmentInDocument : Entity
    {
        public Guid DocumentId { get; set; }

        public Document Document { get; set; }

        public List<KKSRepairDocument> KKSEquipment { get; set; }
    }
}
