using EquipmentRepairLog.Core.Data.DocumentModel;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions.AppException;
using EquipmentRepairLog.Core.FactoryData;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentService(AppDbContext dbContext, DocumentFactroy documentFactroy)
    {
        public async Task AddAllDocumentsAsync(List<DocumentModelCreator> documentsCreator, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание нового комплекта документа(ов)
                var executeRepairDocument = await CreateERDAsync(cancellationToken);

                // Добавление документа и связь его с комплектом документа(ов)
                documentsCreator.ForEach(e => e?.ExecuteRepairDocuments?.Add(executeRepairDocument));

                // Создание документов для добавления в БД
                var documents = await documentFactroy.GetListDocumentAsync(documentsCreator, cancellationToken);
                await dbContext.Documents.AddRangeAsync(documents, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocument(DocumentModelCreator documentsCreator, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
            {
                // Создание нового комплекта документа(ов)
                var executeRepairDocument = await CreateERDAsync(cancellationToken);

                // Добавление документа и связь его с комплектом документа(ов)
                documentsCreator.ExecuteRepairDocuments?.Add(executeRepairDocument);

                // Создание документа для добавления в БД
                var document = await documentFactroy.GetDocumentAsync(documentsCreator, cancellationToken);
                await dbContext.Documents.AddAsync(document, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return DBNull.Value;
            },
            cancellationToken);
        }

        public async Task AddDocumentFromERDAsync(DocumentModelCreator documentsCreator, string registrationNumberDoc, CancellationToken cancellationToken = default)
        {
            await dbContext.RunTransactionAsync(async _ =>
            {
                var document = await documentFactroy.GetDocumentFromERDAsync(documentsCreator, registrationNumberDoc, cancellationToken);
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
                var documents = await documentFactroy.GetRemoveDocsByRegistrationNumberAsync(registrationNumberDoc, cancellationToken);

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
