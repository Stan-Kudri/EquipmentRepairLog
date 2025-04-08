namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class Division : Entity
    {
        //Наименование цеха владельца оборудования
        public string Name { get; set; } = string.Empty;

        //Номер цеха/отдела
        public int Number { get; set; }

        //Аббревиатура владельца оборудования
        public string Abbreviation { get; set; } = string.Empty;
    }
}
