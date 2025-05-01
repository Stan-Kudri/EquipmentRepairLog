using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;

namespace EquipmentRepairLog.Core.Extension
{
    public static class ValidDocuDBExtension
    {
        public static bool ValidListDocFromDB(this AppDbContext dbContext, List<Document> documents)
            => documents.All(dbContext.ValidDataDocumentTypeAndNumber);

        public static bool ValidDataDocumentTypeAndNumber(this AppDbContext dbContext, Document document)
            => dbContext.Documents.FirstOrDefault(e => (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                           || e.RegistrationNumber == document.RegistrationNumber) == null;
    }
}
