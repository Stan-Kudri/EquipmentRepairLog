using EquipmentRepairLog.Core.Data.EquipmentModel;
using EquipmentRepairLog.Core.Data.PerfomerModel;
using EquipmentRepairLog.Core.Data.StandardModel;

namespace EquipmentRepairLog.Core.Data.DocumentModel
{
    public class Document : Entity
    {
        //Дата регистрации
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        //Дата ремонта
        public DateTime RepairDate { get; set; }

        //Дата для изменения
        //(Для изменения регистрационного номера после прошедшего срока или до него)
        public DateTime? ChangeDateRegistrNumber { get; set; } = null;

        //Порядковый номер документа по типу
        //(Зависит от года регистрации,
        //т.е. с каждого года новая нумерация)
        public int OrdinalNumber { get; set; }

        //Примечание
        public string? Note { get; set; } = string.Empty;

        //Регистрационный номер
        public string RegistrNumber { get; set; } = string.Empty;

        public Guid IdTypeDoc { get; set; }

        //Тип документа
        public TypeDocument TypeDocument { get; set; }

        public Guid IdEquipmentInDoc { get; set; }

        //Оборудование к документу
        public EquipmentInDocument Equipments { get; set; }

        public Guid IdDivision { get; set; }

        //Цех/отдел предстовитель оборудования
        public Division Division { get; set; }

        public Guid IdPerfomersInDocument { get; set; }

        //Исполнители работ
        public PerfomerInDocuments Perfomer { get; set; }

        public Guid IdRepairFacility { get; set; }

        //Место нахождения оборудования (По пренадлежности к ЭБ 1, ЭБ 2 и ОСО
        public RepairFacility RepairFacility { get; set; }

        public Guid IdExecuteRepairDoc { get; set; }

        //Номер принадлежности к документам
        public ExecuteRepairDocuments ExecuteRepairDocuments { get; set; }
    }
}
