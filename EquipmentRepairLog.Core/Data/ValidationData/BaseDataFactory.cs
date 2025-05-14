using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Exceptions;
using System.Text;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public abstract class BaseDataFactory<T>
        where T : Entity
    {
        private const byte MaxLengthName = 40;
        private const byte MinLengthName = 6;

        private const byte MaxLengthAbbreviation = 15;
        private const byte MinLengthAbbreviation = 2;

        protected void BaseFieldValidation(ref string name, ref string abbreviation)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentException.ThrowIfNullOrEmpty(abbreviation);

            name = NormalizeString(name);
            abbreviation = NormalizeString(abbreviation);

            if (name.Length is > MaxLengthName or <= MinLengthName)
            {
                throw BusinessLogicException.InvalidFormat<DocumentType>(nameof(name), MinLengthName, MaxLengthName);
            }
            if (abbreviation.Length is > MaxLengthAbbreviation or <= MinLengthAbbreviation)
            {
                throw BusinessLogicException.InvalidFormat<DocumentType>(nameof(abbreviation), MinLengthAbbreviation, MaxLengthAbbreviation);
            }
        }

        protected string NormalizeString(string str)
            => new StringBuilder(str).Replace(" ", "").Replace("-", "").Replace(".", "").Replace(",", "").Replace("/", "").ToString();
    }
}
