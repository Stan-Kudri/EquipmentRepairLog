namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class EquipmentName : Entity
    {
        //Наименование оборудования
        public required string Name { get; set; }

        //Описание (Тип, Марка и другая информация)
        public string Description { get; set; } = string.Empty;

        public List<KKSEquipment>? EquipmentsKKS { get; set; } = null;
    }
}
