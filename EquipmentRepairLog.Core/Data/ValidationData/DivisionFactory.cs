using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Exceptions;
using System.Text;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class DivisionFactory
    {
        public const byte MaxNumberDivision = 100;
        public const byte MinNumberDivision = 1;

        public const byte MaxLengthName = 40;
        public const byte MinLengthName = 6;

        public const byte MaxLengthAbbreviation = 15;
        public const byte MinLengthAbbreviation = 2;

        public Division Create(string name, string abbreviation, byte number)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentException.ThrowIfNullOrEmpty(abbreviation);

            name = NormalizeString(name);
            abbreviation = NormalizeString(abbreviation);

            if (number > MaxNumberDivision || number <= MinNumberDivision)
            {
                throw new EquipmentRepairLogException($"Number division {number} is out of range [{MinNumberDivision}...{MaxNumberDivision}].");
            }
            if (name.Length > MaxLengthName || name.Length <= MinLengthName)
            {
                throw new EquipmentRepairLogException($"Name division {name} is out of range [{MaxLengthName}...{MinLengthName}].");
            }
            if (abbreviation.Length > MaxLengthAbbreviation || abbreviation.Length <= MinLengthAbbreviation)
            {
                throw new EquipmentRepairLogException($"Abbreviation division {abbreviation} is out of range [{MaxLengthAbbreviation}...{MinLengthAbbreviation}].");
            }

            return new Division() { Name = name, Abbreviation = abbreviation, Number = number };
        }

        private string NormalizeString(string str)
            => new StringBuilder(str).Replace(" ", "").Replace("-", "").Replace(".", "").Replace(",", "").Replace("/", "").ToString();
    }
}
