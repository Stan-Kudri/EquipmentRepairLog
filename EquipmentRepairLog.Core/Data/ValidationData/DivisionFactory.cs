using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Exceptions;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class DivisionFactory : BaseDataFactory<Division>
    {
        public const byte MaxNumberDivision = 100;
        public const byte MinNumberDivision = 1;

        public Division Create(string name, string abbreviation, byte number)
        {
            if (number is > MaxNumberDivision or <= MinNumberDivision)
            {
                throw BusinessLogicException.InvalidFormat<Division>(nameof(number), MinNumberDivision, MaxNumberDivision);
            }
            BaseFieldValidation(ref name, ref abbreviation);

            return new Division() { Name = name, Abbreviation = abbreviation, Number = number };
        }
    }
}
