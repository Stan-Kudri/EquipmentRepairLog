using EquipmentRepairDocument.Core.Data.DocumentModel;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions.AppException;
using EquipmentRepairDocument.Core.FactoryData;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairDocument.Core.Service
{
    public class DocumentService(AppDbContext dbContext, DocumentFactory documentFactroy)
    {
        public async Task AddAllDocumentsAsync(List<DocumentCreateRequest> documentCreateRequests, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание нового комплекта документа(ов)
                var executeRepairDocument = await CreateERDAsync(cancellationToken);

                // Добавление документа и связь его с комплектом документа(ов)
                documentCreateRequests.ForEach(e => e?.ExecuteRepairDocumentsId?.Add(executeRepairDocument.Id));

                // Создание документов для добавления в БД
                foreach (var documentRequest in documentCreateRequests)
                {
                    var document = await documentFactroy.CreateDocumentAsync(documentRequest, cancellationToken);
                    await dbContext.Documents.AddAsync(document, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocument(DocumentCreateRequest documentCreateRequest, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание нового комплекта документа(ов)
                var executeRepairDocument = await CreateERDAsync(cancellationToken);

                // Добавление документа и связь его с комплектом документа(ов)
                documentCreateRequest.ExecuteRepairDocumentsId?.Add(executeRepairDocument.Id);

                // Создание документа для добавления в БД
                var document = await documentFactroy.CreateDocumentAsync(documentCreateRequest, cancellationToken);
                await dbContext.Documents.AddAsync(document, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocumentFromERDAsync(DocumentCreateRequest documentCreateRequest, string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
            {
                var document = await documentFactroy.CreateDocumentFromERDAsync(documentCreateRequest, registrationNumberDoc, cancellationToken);
                await dbContext.Documents.AddAsync(document, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task RemoveDocument(Guid id)
        {
            var count = await dbContext.Documents.Where(e => e.Id == id).ExecuteDeleteAsync();
            if (count == 0)
            {
                throw new NotFoundException($"Not found document to delete by '{id}'.");
            }
        }

        public async Task RemoveERDAsync(string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            var executeRepairDocuments = await dbContext.Documents.Include(e => e.ExecuteRepairDocuments)
                                                                  .Where(e => e.RegistrationNumber == registrationNumberDoc)
                                                                  .Select(e => e.ExecuteRepairDocuments)
                                                                  .FirstOrDefaultAsync(cancellationToken)
                                                                  ?? throw new NotFoundException($"Document with registration number \"{registrationNumberDoc}\" not found.");

            await dbContext.RunTransactionAsync(async _ =>
            {
                // Получение комплекта документов для удаления по регистрационному номеру одного документа
                var executeRepairDocumentsId = executeRepairDocuments.ConvertAll(e => e.Id);
                var documents = dbContext.ExecuteRepairDocuments.Include(erd => erd.Documents).Where(e => executeRepairDocumentsId.Contains(e.Id)).SelectMany(e => e.Documents).Distinct();

                dbContext.Documents.RemoveRange(documents);
                dbContext.ExecuteRepairDocuments.RemoveRange(executeRepairDocuments);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        private async Task<ExecuteRepairDocument> CreateERDAsync(CancellationToken cancellationToken = default)
            => (await dbContext.ExecuteRepairDocuments.AddAsync(
                                                                new ExecuteRepairDocument(),
                                                                cancellationToken)
                                                                ).Entity;
    }
}
