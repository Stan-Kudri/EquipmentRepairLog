using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;

namespace EquipmentRepairLog.Core.Extension
{
    public static class ValidDocuDBExtension
    {
        public static bool ValidListDocFromDB(this AppDbContext dbContext, List<Document> documents)
        {
            foreach (var document in documents)
            {
                if (dbContext.Documents.FirstOrDefault(e => e.Id == document.Id
                                                        || (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                        || e.RegistrationNumber == document.RegistrationNumber) != null)
                {
                    return false;
                }

                if (dbContext.DocumentTypes.FirstOrDefault(e => e.Id == document.Id) != null)
                {
                    document.Id = ChangeIdDocumentType(dbContext);
                }
            }

            return true;
        }

        public static bool ValidDocFromDB(this AppDbContext dbContext, Document document)
        {
            if (dbContext.Documents.FirstOrDefault(e => e.Id == document.Id
                                                        || (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                        || e.RegistrationNumber == document.RegistrationNumber) != null)
            {
                return false;
            }

            if (dbContext.DocumentTypes.FirstOrDefault(e => e.Id == document.Id) != null)
            {
                document.Id = ChangeIdDocumentType(dbContext);
            }

            return true;
        }

        private static Guid ChangeIdDocumentType(AppDbContext dbContext)
        {
            var id = Guid.NewGuid();
            return dbContext.Documents.FirstOrDefault(d => d.Id == id) == null ? id : ChangeIdDocumentType(dbContext);
        }
    }
}
