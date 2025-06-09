namespace EquipmentRepairDocument.Core.Data.EquipmentModel
{
    public class EquipmentType : Entity, IEquatable<EquipmentType>
    {
        /// <summary>
        /// Наименование типа и(или) марки.
        /// </summary>
        public required string Name { get; set; }

        public required Guid EquipmentId { get; set; }

        /// <summary>
        /// Наименование оборудования.
        /// </summary>
        public Equipment? Equipment { get; set; }

        public bool Equals(EquipmentType? type)
            => type is not null && type.Name == Name;

        public override int GetHashCode() => HashCode.Combine(Name, Id, EquipmentId);
    }
}
