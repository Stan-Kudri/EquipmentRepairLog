using EquipmentRepairLog.Core.Data.StandardModel;
using EquipmentRepairLog.Core.DBContext;

namespace EquipmentRepairLog.Core.Service
{
    public class DocumentTypeService
    {
        private readonly AppDbContext _dbContext;

        public DocumentTypeService(AppDbContext dbContext)
            => _dbContext = dbContext;

        public void Add(DocumentType documentType)
        {
            if (documentType == null)
            {
                throw new ArgumentNullException("Transmitted data error.", nameof(documentType));
            }
            if (_dbContext.DocumentTypes.FirstOrDefault(e => e.Abbreviation == documentType.Abbreviation
                                                        || e.Name == documentType.Name
                                                        || e.ExecutiveRepairDocNumber == documentType.ExecutiveRepairDocNumber) != null)
            {
                throw new ArgumentException("Data already in use.", nameof(documentType));
            }
            if (_dbContext.DocumentTypes.FirstOrDefault(e => e.Id == documentType.Id) != null)
            {
                documentType.Id = ChangeIdDocumentType();
            }

            _dbContext.DocumentTypes.Add(documentType);
            _dbContext.SaveChanges();
        }

        public void Remove(Guid id)
        {
            var item = _dbContext.DocumentTypes.FirstOrDefault(e => e.Id == id)
                        ?? throw new InvalidOperationException("Interaction element not found.");

            _dbContext.DocumentTypes.Remove(item);
            _dbContext.SaveChanges();
        }

        public DocumentType? GetDocumentType(Guid id)
            => _dbContext.DocumentTypes.FirstOrDefault(e => e.Id == id);

        public DocumentType? GetDocumentType(string abbreviation)
            => _dbContext.DocumentTypes.FirstOrDefault(e => e.Abbreviation == abbreviation);

        private Guid ChangeIdDocumentType()
        {
            var id = Guid.NewGuid();
            return _dbContext.DocumentTypes.FirstOrDefault(d => d.Id == id) == null ? id : ChangeIdDocumentType();
        }
    }
}
