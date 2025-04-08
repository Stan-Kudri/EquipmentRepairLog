namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class RepairFacility : Entity
    {
        //Наименование принадлежности к объекту (ЭБ 1 / ЭБ 2 / ОСО)
        public string Name { get; set; } = string.Empty;

        //Номер объекта
        public int Number { get; set; }
    }
}
