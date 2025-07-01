using EquipmentRepairDocument.Core.Data.ValidationData;

namespace EquipmentRepairDocument.Core.Data.DocumentModel
{
    public class DocumentCreateRequest
    {
        /// <summary>
        /// Дата регистрации.
        /// </summary>
        public required DateTime RegistrationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Дата ремонта.
        /// </summary>
        public required DateTime RepairDate { get; set; }

        /// <summary>
        /// Дата для изменения номера регистрации
        /// (Для изменения регистрационного номера после прошедшего срока или до него).
        /// </summary>
        public DateTime? ChangeDateRegistrNumber { get; set; } = null;

        /// <summary>
        /// Примечание.
        /// </summary>
        public string? Note { get; set; } = string.Empty;

        /// <summary>
        /// Тип документа.
        /// </summary>
        public required Guid DocumentTypeId { get; set; }

        /// <summary>
        /// Цех/отдел предстовитель оборудования.
        /// </summary>
        public required Guid DivisionId { get; set; }

        /// <summary>
        /// Место нахождения оборудования (По принадлежности к ЭБ 1, ЭБ 2 и ОСО).
        /// </summary>
        public required Guid RepairFacilityId { get; set; }

        /// <summary>
        /// Список KKS оборудования.
        /// </summary>
        public List<KKSEquipmentRequest>? KKSEquipment { get; set; } = new List<KKSEquipmentRequest>();

        /// <summary>
        /// Исполнители работ.
        /// </summary>
        public List<Guid>? PerfomersId { get; set; } = new List<Guid>();

        /// <summary>
        /// Список комплектов связанных с документом.
        /// </summary>
        public List<Guid> ExecuteRepairDocumentsId { get; set; } = new List<Guid>();
    }
}
