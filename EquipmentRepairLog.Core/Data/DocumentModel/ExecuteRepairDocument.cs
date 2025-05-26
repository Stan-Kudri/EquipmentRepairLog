namespace EquipmentRepairLog.Core.Data.DocumentModel
{
    public class ExecuteRepairDocument : Entity
    {
        public List<Document> Documents { get; set; } = new List<Document>();
    }
}
