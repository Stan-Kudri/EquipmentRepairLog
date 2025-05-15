using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Exceptions;
using System.Text;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class DocumentTypeFactory
    {
        public const byte MaxExcutiveRepairDocNumber = 100;
        public const byte MinEcutiveRepairDocNumber = 1;

        public const byte MaxLengthName = 40;
        public const byte MinLengthName = 6;

        public const byte MaxLengthAbbreviation = 15;
        public const byte MinLengthAbbreviation = 2;

        public DocumentType Create(string name, string abbreviation, byte executiveRepairDocNumber, bool isOnlyTypeDocInRepairLog)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentException.ThrowIfNullOrEmpty(abbreviation);

            name = NormalizeString(name);
            abbreviation = NormalizeString(abbreviation);

            if (executiveRepairDocNumber > MaxExcutiveRepairDocNumber || executiveRepairDocNumber <= MinEcutiveRepairDocNumber)
            {
                throw new EquipmentRepairLogException($"Number type document {executiveRepairDocNumber} is out of range [{MinEcutiveRepairDocNumber}...{MaxExcutiveRepairDocNumber}].");
            }
            if (name.Length > MaxLengthName || name.Length <= MinLengthName)
            {
                throw new EquipmentRepairLogException($"Name type document {name} is out of range [{MaxLengthName}...{MinLengthName}].");
            }
            if (abbreviation.Length > MaxLengthAbbreviation || abbreviation.Length <= MinLengthAbbreviation)
            {
                throw new EquipmentRepairLogException($"Abbreviation type document {abbreviation} is out of range [{MaxLengthAbbreviation}...{MinLengthAbbreviation}].");
            }

            return new DocumentType() { Name = name, Abbreviation = abbreviation, ExecutiveRepairDocNumber = executiveRepairDocNumber, IsOnlyTypeDocInRepairLog = isOnlyTypeDocInRepairLog };
        }

        private string NormalizeString(string str)
            => new StringBuilder(str).Replace(" ", "").Replace("-", "").Replace(".", "").Replace(",", "").Replace("/", "").ToString();
    }
}
