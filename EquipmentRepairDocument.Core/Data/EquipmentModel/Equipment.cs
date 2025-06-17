namespace EquipmentRepairDocument.Core.Data.EquipmentModel
{
    public class Equipment : Entity, IEquatable<Equipment>
    {
        /// <summary>
        /// Наименование оборудования.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Описание (другая информация).
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Список KKS оборудования.
        /// </summary>
        public List<KKSEquipment>? EquipmentsKKS { get; set; } = new List<KKSEquipment>();

        /// <summary>
        /// Список типов/марок оборудования.
        /// </summary>
        public List<EquipmentType>? EquipmentTypes { get; set; } = new List<EquipmentType>();

        public bool Equals(Equipment? equipment)
            => equipment is not null && equipment.Name == Name;

        public override int GetHashCode() => HashCode.Combine(Name);
    }
}
