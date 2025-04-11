using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.PerfomerModel
{
    public class PerfomerInDocuments : Entity
    {
        public Guid DocumentId { get; set; }

        public Document Document { get; set; }

        public List<PerfomersWork> Perfomers { get; set; }
    }
}
