namespace EquipmentRepairLog.Core.Data.DocumentModel
{
    public class ExecuteRepairDocument : Entity, IEquatable<ExecuteRepairDocument>
    {
        public List<Document> Documents { get; set; } = new List<Document>();

        public bool Equals(ExecuteRepairDocument? executeRepairDocument)
            => executeRepairDocument is not null && executeRepairDocument.Id == Id;
    }
}
