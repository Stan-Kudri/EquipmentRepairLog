using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.Exceptions;

namespace EquipmentRepairDocument.Core.FactoryData
{
    public class DivisionFactory : BaseDataFactory<Division>
    {
        public const byte MaxNumberDivision = 100;
        public const byte MinNumberDivision = 1;

        public Division Create(string name, string abbreviation, byte number)
        {
            BusinessLogicException.EnsureRange<Division>(number, nameof(number), MinNumberDivision, MaxNumberDivision);
            var result = EnsureValid(name, abbreviation);

            return new Division()
            {
                Name = result.Name,
                Abbreviation = result.Abbreviation,
                Number = number,
            };
        }
    }
}
