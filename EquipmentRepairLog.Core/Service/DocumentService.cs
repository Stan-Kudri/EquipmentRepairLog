using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using EquipmentRepairLog.Core.Exceptions.AppException;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentService(AppDbContext dbContext)
    {
        public void AddAllDocuments(List<Document> documents)
        {
            ArgumentNullException.ThrowIfNull(documents);

            if (!IsFreePropertyDocuments(documents))
            {
                var invalidDocuments = documents.Where(document => dbContext.Documents.FirstOrDefault(e
                                                => (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType)
                                                || e.RegistrationNumber == document.RegistrationNumber) == null).Select(document => new { document.RegistrationNumber }).ToList();
                throw new BusinessLogicException($"The document with registration number \"{string.Join(';', invalidDocuments.Select(e => e.RegistrationNumber))}\" is already taken.");
            }

            var transaction = dbContext.Database.BeginTransaction();

            try
            {
                var executeRepairDocument = new ExecuteRepairDocument();
                dbContext.ExecuteRepairDocuments.Add(executeRepairDocument);

                documents.ForEach(e => e?.ExecuteRepairDocuments?.Add(executeRepairDocument));
                dbContext.Documents.AddRange(documents);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new TransactionAppException("Error in save data.", e);
            }
        }

        public void AddDocument(Document document)
        {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(document.ExecuteRepairDocuments);

            if (!IsFreePropertyDocument(document))
            {
                throw new BusinessLogicException($"The document with registration number \"{document.RegistrationNumber}\" and order number \"{document.OrdinalNumber}\" is already taken.");
            }

            var transaction = dbContext.Database.BeginTransaction();

            try
            {
                //Создание нового комплекта документа(ов)
                var executeRepairDocument = new ExecuteRepairDocument();
                dbContext.ExecuteRepairDocuments.Add(executeRepairDocument);

                //Добавление документа и связь его с комплектом документа(ов)
                document.ExecuteRepairDocuments.Add(executeRepairDocument);
                dbContext.Documents.Add(document);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new TransactionAppException("Error in save data.", e);
            }
        }

        public void AddDocumentFromERD(Document document, string registrationNumberDoc)
        {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(document.ExecuteRepairDocuments);

            if (!IsFreePropertyDocument(document))
            {
                throw new BusinessLogicException($"The document with registration number \"{document.RegistrationNumber}\" and order number \"{document.OrdinalNumber}\" is already taken.");
            }

            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments).FirstOrDefault(e => e.RegistrationNumber == registrationNumberDoc)
                                                                    ?? throw new BusinessLogicException($"Registration number {registrationNumberDoc} not found.");

            var executeRepairDoc = docByRegistrationNumber?.ExecuteRepairDocuments?.FirstOrDefault()
                                    ?? throw new BusinessLogicException("Empty Execute Repair Document.");

            var transaction = dbContext.Database.BeginTransaction();

            try
            {
                document.ExecuteRepairDocuments.Add(executeRepairDoc);
                dbContext.Documents.Add(document);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new TransactionAppException("Error in save data.", e);
            }
        }

        public void RemoveDocument(Guid id)
        {
            var count = dbContext.Documents.Where(e => e.Id == id).ExecuteDelete();
            if (count == 0)
            {
                throw new NotFoundException($"Not found document to delete by '{id}'.");
            }
        }

        public void RemoveERD(string registrationNumberDoc)
        {
            var docByRegistrationNumber = dbContext.Documents.Include(e => e.ExecuteRepairDocuments)
                                                             .Where(e => e.RegistrationNumber == registrationNumberDoc)
                                                             .Select(e => new { e.ExecuteRepairDocuments, })
                                                             .FirstOrDefault();

            if (docByRegistrationNumber == null)
            {
                throw new NotFoundException($"Document with registration number \"{registrationNumberDoc}\" not found.");
            }
            if (docByRegistrationNumber.ExecuteRepairDocuments != null && docByRegistrationNumber.ExecuteRepairDocuments.Count == 0)
            {
                throw new BusinessLogicException($"Document with registration number {registrationNumberDoc} does not belong to any set of executive repair documentation.");
            }

            var executeRepairDocuments = docByRegistrationNumber.ExecuteRepairDocuments;
            ArgumentNullException.ThrowIfNull(executeRepairDocuments);

            var transaction = dbContext.Database.BeginTransaction();

            try
            {
                var executeRepairDocumentsId = executeRepairDocuments.ConvertAll(e => e.Id);
                var documents = dbContext.ExecuteRepairDocuments.Include(erd => erd.Documents).Where(e => executeRepairDocumentsId.Contains(e.Id)).SelectMany(e => e.Documents).Distinct();

                dbContext.Documents.RemoveRange(documents);
                dbContext.ExecuteRepairDocuments.RemoveRange(executeRepairDocuments);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new TransactionAppException("Error in save data.", e);
            }
        }

        private bool IsFreePropertyDocuments(List<Document> documents)
        {
            if (documents.Any())
            {
                return true;
            }

            var registrationNumber = documents.Select(e => e.RegistrationNumber);
            var numberDocument = documents.Select(e => new { e.OrdinalNumber, e.DocumentType });

            return !dbContext.Documents.Any(document => registrationNumber.Contains(document.RegistrationNumber)
                                                        || numberDocument.Any(e => e.OrdinalNumber == document.OrdinalNumber
                                                                                   && e.DocumentType == document.DocumentType));
        }

        private bool IsFreePropertyDocument(Document document)
            => !dbContext.Documents.Any(e => e.RegistrationNumber == document.RegistrationNumber
                                            || (e.OrdinalNumber == document.OrdinalNumber && e.DocumentType == document.DocumentType));
    }
}
