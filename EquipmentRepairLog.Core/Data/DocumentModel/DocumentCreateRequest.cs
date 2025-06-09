using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;

namespace EquipmentRepairLog.Core.Data.DocumentModel
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

        public required Guid DocumentTypeId { get; set; }

        /// <summary>
        /// Тип документа.
        /// </summary>
        public DocumentType? DocumentType { get; set; }

        public required Guid DivisionId { get; set; }

        /// <summary>
        /// Цех/отдел предстовитель оборудования.
        /// </summary>
        public Division? Division { get; set; }

        public required Guid RepairFacilityId { get; set; }

        /// <summary>
        /// Место нахождения оборудования (По принадлежности к ЭБ 1, ЭБ 2 и ОСО).
        /// </summary>
        public RepairFacility? RepairFacility { get; set; }

        /// <summary>
        /// Список KKS оборудования.
        /// </summary>
        public List<KKSEquipment>? KKSEquipment { get; set; } = new List<KKSEquipment>();

        /// <summary>
        /// Исполнители работ.
        /// </summary>
        public List<Perfomer>? Perfomers { get; set; } = new List<Perfomer>();

        /// <summary>
        /// Список комплектов связанных с документом.
        /// </summary>
        public List<ExecuteRepairDocument> ExecuteRepairDocuments { get; set; } = new List<ExecuteRepairDocument>();
    }
}
