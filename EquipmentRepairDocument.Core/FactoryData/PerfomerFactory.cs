using EquipmentRepairDocument.Core.Data.StandardModel;

namespace EquipmentRepairDocument.Core.FactoryData
{
    public class PerfomerFactory : BaseDataFactory<Division>
    {
        public Perfomer Create(string name, string abbreviation)
        {
            var result = EnsureValid(name, abbreviation);
            return new Perfomer() { Name = result.Name, Abbreviation = result.Abbreviation, };
        }
    }
}
