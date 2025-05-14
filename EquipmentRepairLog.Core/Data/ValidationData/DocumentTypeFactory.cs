using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.Exceptions;

namespace EquipmentRepairLog.Core.Data.ValidationData
{
    public class DocumentTypeFactory : BaseDataFactory<Division>
    {
        public const byte MaxExcutiveRepairDocNumber = 100;
        public const byte MinEcutiveRepairDocNumber = 1;

        public DocumentType Create(string name, string abbreviation, byte executiveRepairDocNumber, bool isOnlyTypeDocInRepairLog)
        {
            if (executiveRepairDocNumber is > MaxExcutiveRepairDocNumber or <= MinEcutiveRepairDocNumber)
            {
                throw BusinessLogicException.InvalidFormat<DocumentType>(nameof(executiveRepairDocNumber), MinEcutiveRepairDocNumber, MaxExcutiveRepairDocNumber);
            }
            BaseFieldValidation(ref name, ref abbreviation);

            return new DocumentType() { Name = name, Abbreviation = abbreviation, ExecutiveRepairDocNumber = executiveRepairDocNumber, IsOnlyTypeDocInRepairLog = isOnlyTypeDocInRepairLog };
        }
    }
}
