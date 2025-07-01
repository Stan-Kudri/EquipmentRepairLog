using EquipmentRepairDocument.Core.Data.DocumentModel;

namespace EquipmentRepairDocument.Core.Data.StandardModel
{
    public class DocumentType : Entity, IEquatable<DocumentType>
    {
        /// <summary>
        /// Название документа.
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Аббревиатура типа документа.
        /// </summary>
        public required string Abbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Номер документа ИРД согласно СТО.
        /// </summary>
        public required byte ExecutiveRepairDocNumber { get; set; }

        /// <summary>
        /// Флаг едиственного типа документа к комплекту ИРД.
        /// </summary>
        public required bool MultipleUseInERD { get; set; }

        /// <summary>
        /// Список документов для данного типа документа.
        /// </summary>
        public List<Document>? Documents { get; set; } = new List<Document>();

        public bool Equals(DocumentType? documentType)
            => documentType is not null && documentType.Name == Name && documentType.Abbreviation == Abbreviation && documentType.ExecutiveRepairDocNumber == ExecutiveRepairDocNumber;

        public override int GetHashCode() => HashCode.Combine(Name, Abbreviation, ExecutiveRepairDocNumber);
    }
}
