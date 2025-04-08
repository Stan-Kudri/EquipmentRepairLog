namespace EquipmentRepairLog.Core.Data.DocumentModel
{
    public class ExecuteRepairDocuments : Entity
    {
        public List<Document> Documents { get; set; } = new List<Document>();
    }
}
