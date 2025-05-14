using EquipmentRepairLog.Core.Data.DocumentModel;
using System.ComponentModel.DataAnnotations;

namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class KKSEquipment : Entity
    {
        /// <summary>
        /// Идентификационный номер оборудования/детали
        /// </summary>
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only Latin letters and numbers are allowed.")]
        public required string KKS { get; set; } = string.Empty;

        public required Guid EquipmentId { get; set; } = Guid.Empty;

        public required Guid EquipmentTypeId { get; set; } = Guid.Empty;

        /// <summary>
        /// Наименование оборудования
        /// </summary>
        public Equipment? Equipment { get; set; }

        /// <summary>
        /// Тип и(или) Марка оборудования
        /// </summary>
        public EquipmentType? EquipmentType { get; set; }

        /// <summary>
        /// Список документов для данного KKS
        /// </summary>
        public List<Document>? KKSEquipmentDocuments { get; set; } = new List<Document>();

        public bool Equals(KKSEquipment? kksEquipment)
            => kksEquipment is not null && kksEquipment.KKS == KKS && kksEquipment.EquipmentId == EquipmentId && kksEquipment.EquipmentTypeId == EquipmentTypeId;

        public override int GetHashCode() => HashCode.Combine(KKS, EquipmentId, EquipmentTypeId);
    }
}
