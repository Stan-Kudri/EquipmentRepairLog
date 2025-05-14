using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class RepairFacility : Entity
    {
        /// <summary>
        /// Наименование принадлежности к объекту (ЭБ 1 / ЭБ 2 / ОСО)
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Аббревиатура объекта
        /// </summary>
        public required string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Номер объекта
        /// </summary>
        public required byte Number { get; set; }

        /// <summary>
        /// Список документов по объекту
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();

        public bool Equals(RepairFacility? repairFacility)
            => repairFacility is not null && repairFacility.Name == Name && repairFacility.Abbreviation == Abbreviation && repairFacility.Number == Number;

        public override int GetHashCode() => HashCode.Combine(Name, Abbreviation, Number);
    }
}
