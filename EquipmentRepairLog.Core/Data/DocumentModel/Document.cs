using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.StandardModel;

namespace EquipmentRepairLog.Core.Data.DocumentModel
{
    public class Document : Entity
    {
        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Дата ремонта
        /// </summary>
        public DateTime RepairDate { get; set; }

        /// <summary>
        /// Дата для изменения номера регистрации
        /// (Для изменения регистрационного номера после прошедшего срока или до него)
        /// </summary>
        public DateTime? ChangeDateRegistrNumber { get; set; } = null;

        /// <summary>
        /// Порядковый номер документа по типу и году регистрации
        /// </summary>
        public int OrdinalNumber { get; set; }

        /// <summary>
        /// Регистрационный номер документа
        /// </summary>
        public string RegistrationNumber { get; set; } = string.Empty;

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; set; } = string.Empty;

        public Guid DocumentTypeId { get; set; }

        /// <summary>
        /// Тип документа
        /// </summary>
        public DocumentType? DocumentType { get; set; }

        public Guid DivisionId { get; set; }

        /// <summary>
        /// Цех/отдел предстовитель оборудования
        /// </summary>
        public Division? Division { get; set; }

        public Guid RepairFacilityId { get; set; }

        /// <summary>
        /// Место нахождения оборудования (По принадлежности к ЭБ 1, ЭБ 2 и ОСО)
        /// </summary>
        public RepairFacility? RepairFacility { get; set; }

        /// <summary>
        /// Список регистрационных номеров связанных документовццт
        /// </summary>
        public List<Document>? Documents { get; set; }

        /// <summary>
        /// Список KKS оборудования
        /// </summary>
        public List<KKSEquipment>? KKSEquipment { get; set; }

        /// <summary>
        /// Исполнители работ
        /// </summary>
        public List<Perfomer>? Perfomers { get; set; }
    }
}
