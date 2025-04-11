namespace EquipmentRepairLog.Core.Data.PerfomerModel
{
    public class PerfomersWork : Entity
    {
        public Guid PerfomerId { get; set; }

        public Guid PerfomerInDocId { get; set; }

        public Perfomer Perfomer { get; set; }

        public PerfomerInDocuments PerfomerInDocuments { get; set; }
    }
}
