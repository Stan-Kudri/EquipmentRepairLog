using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentService(AppDbContext dbContext)
    {
        public void AddAllDocuments(List<Document> documents)
        {
            ArgumentNullException.ThrowIfNull(documents);

            if (!DocumentsValidationDataDocumentTypeAndNumber(documents))
            {
                throw new ArgumentException("Data already in use.", nameof(documents));
            }

            var executeRepairDocument = new ExecuteRepairDocument();
            dbContext.ExecuteRepairDocuments.Add(executeRepairDocument);

            documents.ForEach(e => e?.ExecuteRepairDocuments?.Add(executeRepairDocument));
            dbContext.Documents.AddRange(documents);

            dbContext.SaveChanges();
        }

        public void AddDocument(Document document)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (!DocValidDataDocumentTypeAndNumber(document))
            {
                throw new ArgumentException("Data already in use.", nameof(document));
            }

            var executeRepairDocument = new ExecuteRepairDocument();
            dbContext.ExecuteRepairDocuments.Add(executeRepairDocument);

            document?.ExecuteRepairDocuments?.Add(executeRepairDocument);
            dbContext.Documents.Add(document);

            dbContext.SaveChanges();
        }

        public void AddDocumentFromERD(Document document, string registrationNumberDoc)
        {
            ArgumentNullException.ThrowIfNull(document);

            if (!DocValidDataDocumentTypeAndNumber(document))
            {
                throw new ArgumentException("Data already in use.", nameof(document));
            }

            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments).FirstOrDefault(e => e.RegistrationNumber == registrationNumberDoc)
                                                   ?? throw new ArgumentException("Registration number not found.", nameof(registrationNumberDoc));

            var executeRepairDocInRegistrDoc = docByRegistrationNumber?.ExecuteRepairDocuments?.FirstOrDefault()
                                                                      ?? throw new ArgumentException("Empty Execute Repair Document.");


            document?.ExecuteRepairDocuments?.Add(executeRepairDocInRegistrDoc);
            dbContext.Documents.Add(document);

            dbContext.SaveChanges();
        }

        public void RemoveDocument(Guid id)
        {
            var count = dbContext.Documents.Where(e => e.Id == id).ExecuteDelete();
            if (count == 0)
            {
                throw new InvalidOperationException($"Not found document to delete by '{id}'.");
            }
        }

        public void RemoveERD(string registrationNumberDoc)
        {
            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments)
                                                             .Where(e => e.RegistrationNumber == registrationNumberDoc)
                                                             .Select(e => new { e.ExecuteRepairDocuments, })
                                                             .First();

            var executeRepairDocuments = docByRegistrationNumber?.ExecuteRepairDocuments == null || docByRegistrationNumber?.ExecuteRepairDocuments?.Count != 0
                                                ? docByRegistrationNumber?.ExecuteRepairDocuments
                                                : throw new ArgumentException($"Document with registration number {registrationNumberDoc} does not belong to any set of executive repair documentation.");

            var executeRepairDocumentsId = executeRepairDocuments.ConvertAll(e => e.Id);

            var documents = dbContext.ExecuteRepairDocuments.Include(erd => erd.Documents).Where(e => executeRepairDocumentsId.Contains(e.Id)).SelectMany(e => e.Documents).Distinct();

            dbContext.Documents.RemoveRange(documents);
            dbContext.ExecuteRepairDocuments.RemoveRange(executeRepairDocuments);

            dbContext.SaveChanges();
        }

        public Document? GetDocument(Guid id)
            => dbContext.Documents.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public Document? GetDocument(string registrationNumber)
            => dbContext.Documents.AsNoTracking().FirstOrDefault(e => e.RegistrationNumber == registrationNumber);

        public bool DocumentsValidationDataDocumentTypeAndNumber(List<Document> documents)
            => documents.All(DocValidDataDocumentTypeAndNumber);

        public bool DocValidDataDocumentTypeAndNumber(Document document)
            => dbContext.Documents.FirstOrDefault(e => (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                           || e.RegistrationNumber == document.RegistrationNumber) == null;
    }
}
