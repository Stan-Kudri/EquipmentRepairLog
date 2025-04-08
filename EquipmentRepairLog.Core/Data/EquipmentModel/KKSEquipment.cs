using System.ComponentModel.DataAnnotations;

namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class KKSEquipment : Entity
    {
        //Идентификационный номер оборудования/детали
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only Latin letters and numbers are allowed.")]
        public string KKS { get; set; } = string.Empty;

        public Guid IdEquipmentName { get; set; } = Guid.Empty;

        public required EquipmentName EquipmentName { get; set; }

        public List<Equipment> Equipments { get; set; }
    }
}
