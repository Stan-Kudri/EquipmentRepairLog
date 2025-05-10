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

        public override bool Equals(object obj) => Equals(obj as Equipment);

        public bool Equals(Equipment equipment)
            => equipment != null && equipment.Description == Description && equipment.Name == Name;
    }
}
