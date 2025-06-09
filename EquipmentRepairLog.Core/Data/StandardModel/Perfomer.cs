using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class Perfomer : Entity, IEquatable<Perfomer>
    {
        /// <summary>
        /// Название организации/цеха исполнителя работ.
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Аббревиатура исполнителя работ.
        /// </summary>
        public required string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Список документов по исполнителю работ.
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();

        public bool Equals(Perfomer? perfomer)
            => perfomer is not null && perfomer.Name == Name && perfomer.Abbreviation == Abbreviation;

        public override int GetHashCode() => HashCode.Combine(Name, Abbreviation);
    }
}
