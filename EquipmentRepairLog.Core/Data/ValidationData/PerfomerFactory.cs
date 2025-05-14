using EquipmentRepairLog.Core.Data.StandardModel;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class PerfomerFactory : BaseDataFactory<Division>
    {
        public Perfomer Create(string name, string abbreviation)
        {
            BaseFieldValidation(ref name, ref abbreviation);
            return new Perfomer() { Name = name, Abbreviation = abbreviation };
        }
    }
}
