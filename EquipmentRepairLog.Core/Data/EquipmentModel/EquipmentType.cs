namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class EquipmentType : Entity
    {
        /// <summary>
        /// Наименование типа и(или) марки
        /// </summary>
        public required string Name { get; set; }

        public Guid EquipmentId { get; set; }

        /// <summary>
        /// Наименование оборудования
        /// </summary>
        public Equipment? Equipment { get; set; }

        public override bool Equals(object obj) => Equals(obj as EquipmentType);

        public bool Equals(EquipmentType type)
            => type != null && type.Name == Name;
    }
}
