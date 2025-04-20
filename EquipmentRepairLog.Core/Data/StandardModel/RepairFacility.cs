using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class RepairFacility : Entity
    {
        /// <summary>
        /// Наименование принадлежности к объекту (ЭБ 1 / ЭБ 2 / ОСО)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Аббревиатура объекта
        /// </summary>
        public string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Номер объекта
        /// </summary>
        public byte Number { get; set; }

        /// <summary>
        /// Список документов по объекту
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();
    }
}
