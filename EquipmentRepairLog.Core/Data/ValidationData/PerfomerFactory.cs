using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Exceptions;
using System.Text;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class PerfomerFactory
    {
        public const byte MaxLengthName = 30;
        public const byte MinLengthName = 6;

        public const byte MaxLengthAbbreviation = 15;
        public const byte MinLengthAbbreviation = 2;

        public Perfomer Create(string name, string abbreviation)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentException.ThrowIfNullOrEmpty(abbreviation);

            name = NormalizeString(name);
            abbreviation = NormalizeString(abbreviation);

            if (name.Length > MaxLengthName || name.Length <= MinLengthName)
            {
                throw new EquipmentRepairLogException($"Name perfomer {name} is out of range [{MaxLengthName}...{MinLengthName}].");
            }
            if (abbreviation.Length > MaxLengthAbbreviation || abbreviation.Length <= MinLengthAbbreviation)
            {
                throw new EquipmentRepairLogException($"Abbreviation perfomer {abbreviation} is out of range [{MaxLengthAbbreviation}...{MinLengthAbbreviation}].");
            }

            return new Perfomer() { Name = name, Abbreviation = abbreviation };
        }

        private string NormalizeString(string str)
            => new StringBuilder(str).Replace(" ", "").Replace("-", "").Replace(".", "").Replace(",", "").Replace("/", "").ToString();
    }
}
