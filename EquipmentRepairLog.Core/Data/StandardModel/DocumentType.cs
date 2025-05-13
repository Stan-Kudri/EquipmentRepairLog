using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class DocumentType : Entity
    {
        /// <summary>
        /// Название документа
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Аббревиатура типа документа
        /// </summary>
        public required string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Номер документа ИРД согласно СТО
        /// </summary>
        public required byte ExecutiveRepairDocNumber { get; set; }

        /// <summary>
        /// Флаг едиственного типа документа к комплекту ИРД
        /// </summary>
        public required bool IsOnlyTypeDocInRepairLog { get; set; }

        /// <summary>
        /// Список документов для данного типа документа
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();
    }
}
