namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class Equipment : Entity
    {
        /// <summary>
        /// Наименование оборудования
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Описание (другая информация)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Список KKS оборудования
        /// </summary>
        public List<KKSEquipment>? EquipmentsKKS { get; set; } = new List<KKSEquipment>();

        /// <summary>
        /// Список типов/марок оборудования
        /// </summary>
        public List<EquipmentType>? EquipmentTypes { get; set; } = new List<EquipmentType>();
    }
}
