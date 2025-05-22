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
            BusinessLogicException.EnsureRange<Division>(executiveRepairDocNumber, nameof(executiveRepairDocNumber), MinEcutiveRepairDocNumber, MaxExcutiveRepairDocNumber);
            var result = EnsureValid(name, abbreviation);

            return new DocumentType() { Name = result.Name, Abbreviation = result.Abbreviation, ExecutiveRepairDocNumber = executiveRepairDocNumber, IsOnlyTypeDocInRepairLog = isOnlyTypeDocInRepairLog };
        }
    }
}
