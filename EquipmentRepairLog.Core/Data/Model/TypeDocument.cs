namespace EquipmentRepairLog.Core.Data.Model
{
    public class TypeDocument : Entity
    {
        public string Name { get; set; } = string.Empty;

        public int NumberType { get; set; }

        public bool IsOnlyTypeDocInRepairLog { get; set; }
    }
}
