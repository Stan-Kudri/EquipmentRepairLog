using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class Division : Entity, IEquatable<Division>
    {
        /// <summary>
        /// Наименование цеха владельца оборудования.
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Номер цеха/отдела.
        /// </summary>
        public required byte Number { get; set; }

        /// <summary>
        /// Аббревиатура владельца оборудования.
        /// </summary>
        public required string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Список документов для данного цеха/отдела.
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();

        public bool Equals(Division? division)
            => division is not null && division.Name == Name && division.Number == Number && division.Abbreviation == Abbreviation;

        public override int GetHashCode() => HashCode.Combine(Name, Id);
    }
}
