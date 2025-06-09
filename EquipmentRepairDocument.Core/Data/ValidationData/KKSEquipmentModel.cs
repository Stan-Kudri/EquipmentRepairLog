using System.ComponentModel.DataAnnotations;
using EquipmentRepairDocument.Core.Data.DocumentModel;
using EquipmentRepairDocument.Core.Data.EquipmentModel;

namespace EquipmentRepairDocument.Core.Data.ValidationData
{
    public class KKSEquipmentModel
    {
        /// <summary>
        /// Идентификационный номер оборудования/детали.
        /// </summary>
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only Latin letters and numbers are allowed.")]
        public required string KKS { get; set; } = string.Empty;

        public Guid EquipmentId { get; set; } = Guid.Empty;

        public Guid EquipmentTypeId { get; set; } = Guid.Empty;

        /// <summary>
        /// Наименование оборудования.
        /// </summary>
        public Equipment Equipment { get; set; } = null!;

        /// <summary>
        /// Тип и(или) Марка оборудования.
        /// </summary>
        public EquipmentType EquipmentType { get; set; } = null!;

        /// <summary>
        /// Список документов для данного KKS.
        /// </summary>
        public List<Document>? KKSEquipmentDocuments { get; set; } = new List<Document>();
    }
}
