using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Extension;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentService
    {
        private readonly AppDbContext _dbContext;

        public DocumentService(AppDbContext dbContext)
            => _dbContext = dbContext;

        public void AddAllDocuments(List<Document> documents)
        {
            if (documents == null)
            {
                throw new ArgumentNullException("Transmitted data error.", nameof(documents));
            }

            if (!_dbContext.ValidListDocFromDB(documents))
            {
                throw new ArgumentException("Data already in use.", nameof(documents));
            }

            var executeRepairDocument = new ExecuteRepairDocument();
            _dbContext.ExecuteRepairDocuments.Add(executeRepairDocument);

            documents.ForEach(e => e?.ExecuteRepairDocuments?.Add(executeRepairDocument));
            _dbContext.Documents.AddRange(documents);

            _dbContext.SaveChanges();
        }

        public void AddDocument(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Transmitted data error.", nameof(document));
            }

            if (!_dbContext.ValidDocFromDB(document))
            {
                throw new ArgumentException("Data already in use.", nameof(document));
            }

            var executeRepairDocument = new ExecuteRepairDocument();
            _dbContext.ExecuteRepairDocuments.Add(executeRepairDocument);

            document?.ExecuteRepairDocuments?.Add(executeRepairDocument);
            _dbContext.Documents.Add(document);

            _dbContext.SaveChanges();
        }

        public void AddDocumentFromERD(Document document, string registrationNumberDoc)
        {
            if (document == null)
            {
                throw new ArgumentNullException("Transmitted data error.", nameof(document));
            }

            if (!_dbContext.ValidDocFromDB(document))
            {
                throw new ArgumentException("Data already in use.", nameof(document));
            }

            var documentByRegistrationNumber = _dbContext.Documents.FirstOrDefault(e => e.RegistrationNumber == registrationNumberDoc)
                                                                    ?? throw new ArgumentException("Registration number not found.", nameof(registrationNumberDoc));
            var executeRepairDocInRegistrDoc = documentByRegistrationNumber?.ExecuteRepairDocuments?.Count != 0
                                                ? documentByRegistrationNumber?.ExecuteRepairDocuments
                                                : throw new ArgumentException("Empty Execute Repair Document.");


            document?.ExecuteRepairDocuments?.AddRange(executeRepairDocInRegistrDoc);
            _dbContext.Documents.Add(document);

            _dbContext.SaveChanges();
        }

        public void RemoveDocument(Guid id)
        {
            var item = _dbContext.Documents.FirstOrDefault(e => e.Id == id)
                        ?? throw new InvalidOperationException("Interaction element not found.");

            _dbContext.Documents.Remove(item);
            _dbContext.SaveChanges();
        }

        public Document? GetDocumentType(Guid id)
            => _dbContext.Documents.FirstOrDefault(e => e.Id == id);

        public Document? GetDocumentType(string registrationNumber)
            => _dbContext.Documents.FirstOrDefault(e => e.RegistrationNumber == registrationNumber);

        private Guid ChangeIdDocumentType()
        {
            var id = Guid.NewGuid();
            return _dbContext.Documents.FirstOrDefault(d => d.Id == id) == null ? id : ChangeIdDocumentType();
        }
    }
}
