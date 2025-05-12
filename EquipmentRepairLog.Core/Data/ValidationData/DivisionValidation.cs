using EquipmentRepairLog.Core.Data.StandardModel;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class DivisionValidation
    {
        public DivisionValidation(Division division)
        {
            ArgumentException.ThrowIfNullOrEmpty(division.Name, division.Abbreviation);

            var name = division.Name.Trim(' ', '-', '.', ',');
            var abbreviation = division.Abbreviation.Trim(' ', '-', '.', ',');
            var number = division.Number;

            if (number <= 0 || number > 100)
            {
                throw new ArgumentException($"Number division {number} is out of range [1...100].");
            }
            if (name.Length > 30 || name.Length <= 2)
            {
                throw new ArgumentException($"Name division {name} is out of range [2...30].");
            }
            if (abbreviation.Length > 15 || abbreviation.Length <= 1)
            {
                throw new ArgumentException($"Abbreviation division {abbreviation} is out of range [1...15].");
            }

            Value = new Division() { Name = name, Abbreviation = abbreviation, Number = number };
        }

        public Division Value { get; set; }
    }
}
