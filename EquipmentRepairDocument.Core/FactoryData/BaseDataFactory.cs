using System.Text;
using EquipmentRepairDocument.Core.Data;
using EquipmentRepairDocument.Core.Exceptions;

namespace EquipmentRepairDocument.Core.FactoryData
{
    public abstract class BaseDataFactory<T>
        where T : Entity
    {
        private const byte MaxLengthName = 40;
        private const byte MinLengthName = 6;

        private const byte MaxLengthAbbreviation = 15;
        private const byte MinLengthAbbreviation = 2;

        protected (string Name, string Abbreviation) EnsureValid(string name, string abbreviation)
        {
            BusinessLogicException.ThrowIfNullOrEmpty(name);
            BusinessLogicException.ThrowIfNullOrEmpty(abbreviation);

            name = NormalizeString(name);
            abbreviation = NormalizeString(abbreviation);

            BusinessLogicException.EnsureLength<T>(name, nameof(name), MinLengthName, MaxLengthName);
            BusinessLogicException.EnsureLength<T>(abbreviation, nameof(abbreviation), MinLengthAbbreviation, MaxLengthAbbreviation);

            return (name, abbreviation);
        }

        protected string NormalizeString(string str)
            => new StringBuilder(str).Replace(" ", string.Empty)
                                     .Replace("-", string.Empty)
                                     .Replace(".", string.Empty)
                                     .Replace(",", string.Empty)
                                     .Replace("/", string.Empty)
                                     .ToString();
    }
}
