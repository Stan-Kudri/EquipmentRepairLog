using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class Division : Entity
    {
        /// <summary>
        /// Наименование цеха владельца оборудования
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Номер цеха/отдела
        /// </summary>
        public required byte Number { get; set; }

        /// <summary>
        /// Аббревиатура владельца оборудования
        /// </summary>
        public required string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Список документов для данного цеха/отдела
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();
    }
}
