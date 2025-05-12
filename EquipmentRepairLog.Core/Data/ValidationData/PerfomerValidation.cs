using EquipmentRepairLog.Core.Data.StandardModel;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class PerfomerValidation
    {
        public PerfomerValidation(Perfomer perfomer)
        {
            ArgumentException.ThrowIfNullOrEmpty(perfomer.Name, perfomer.Abbreviation);

            var name = perfomer.Name.Trim(' ', '-', '.', ',');
            var abbreviation = perfomer.Abbreviation.Trim(' ', '-', '.', ',');

            if (name.Length > 30 || name.Length <= 2)
            {
                throw new ArgumentException($"Name perfomer {name} is out of range [2...30].");
            }
            if (abbreviation.Length > 15 || abbreviation.Length <= 1)
            {
                throw new ArgumentException($"Abbreviation perfomer {abbreviation} is out of range [1...15].");
            }

            Value = new Perfomer() { Name = name, Abbreviation = abbreviation };
        }

        public Perfomer Value { get; set; }
    }
}
