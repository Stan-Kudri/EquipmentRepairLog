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
                throw new EquipmentRepairLogException(number, typeof(Division), MinNumberDivision, MaxNumberDivision);
                throw new EquipmentRepairLogException($"Number division {number} is out of range [{MinNumberDivision}...{MaxNumberDivision}].");
            }
            if (name.Length > MaxLengthName || name.Length <= MinLengthName)
            {
                throw new EquipmentRepairLogException(name, typeof(Division), MinLengthName, MaxLengthName);
                throw new EquipmentRepairLogException($"Name division {name} is out of range [{MinLengthName}...{MaxLengthName}].");
            }
            if (abbreviation.Length > MaxLengthAbbreviation || abbreviation.Length <= MinLengthAbbreviation)
            {
                throw new EquipmentRepairLogException(abbreviation, typeof(Division), MinLengthAbbreviation, MaxLengthAbbreviation);
                throw new EquipmentRepairLogException($"Abbreviation division {abbreviation} is out of range [{MinLengthAbbreviation}...{MaxLengthAbbreviation}].");
            }

            return new Division() { Name = name, Abbreviation = abbreviation, Number = number };
        }

        private string NormalizeString(string str)
            => new StringBuilder(str).Replace(" ", "").Replace("-", "").Replace(".", "").Replace(",", "").Replace("/", "").ToString();
    }
}
