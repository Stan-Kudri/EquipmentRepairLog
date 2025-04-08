namespace EquipmentRepairLog.Core.Data.PerfomerModel
{
    public class Perfomer : Entity
    {
        //Название организации/цеха исполнителя работ
        public string Name { get; set; } = string.Empty;

        //Аббревиатура исполнителя работ
        public string Abbreviation { get; set; } = string.Empty;
    }
}
