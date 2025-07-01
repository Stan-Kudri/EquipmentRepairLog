using EquipmentRepairDocument.Core.Data.StandardModel;
using EquipmentRepairDocument.Core.Exceptions;

namespace EquipmentRepairDocument.Core.FactoryData
{
    public class DocumentTypeFactory : BaseDataFactory<Division>
    {
        public const byte MaxExcutiveRepairDocNumber = 100;
        public const byte MinEcutiveRepairDocNumber = 1;

        public DocumentType Create(string name, string abbreviation, byte executiveRepairDocNumber, bool isOnlyTypeDocInERD)
        {
            BusinessLogicException.EnsureRange<Division>(executiveRepairDocNumber, nameof(executiveRepairDocNumber), MinEcutiveRepairDocNumber, MaxExcutiveRepairDocNumber);
            var result = EnsureValid(name, abbreviation);

            return new DocumentType()
            {
                Name = result.Name,
                Abbreviation = result.Abbreviation,
                ExecutiveRepairDocNumber = executiveRepairDocNumber,
                MultipleUseInERD = isOnlyTypeDocInERD,
            };
        }
    }
}
