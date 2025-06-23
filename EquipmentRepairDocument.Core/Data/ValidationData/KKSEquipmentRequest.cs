using System.ComponentModel.DataAnnotations;

namespace EquipmentRepairDocument.Core.Data.ValidationData
{
    public class KKSEquipmentRequest
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
        public required string Equipment { get; set; } = string.Empty;

        /// <summary>
        /// Тип и(или) Марка оборудования.
        /// </summary>
        public required string EquipmentType { get; set; } = string.Empty;

        public bool Equals(KKSEquipmentRequest? kksEquipment)
            => kksEquipment is not null && kksEquipment.KKS == KKS && kksEquipment.EquipmentType == EquipmentType;

        public override int GetHashCode() => HashCode.Combine(KKS, EquipmentType);
    }
}
