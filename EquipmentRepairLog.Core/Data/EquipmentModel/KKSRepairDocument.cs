namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class KKSRepairDocument : Entity
    {
        public Guid KKSId { get; set; }

        public Guid EquipmentInDocId { get; set; }

        public KKSEquipment KKSEquipment { get; set; }

        public EquipmentInDocument EquipmentInDoc { get; set; }
    }
}
