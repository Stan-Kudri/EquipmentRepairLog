using EquipmentRepairLog.Core.Data.DocumentModel;

namespace EquipmentRepairLog.Core.Data.StandardModel
{
    public class TypeDocument : Entity
    {
        //Название документа
        public string Name { get; set; } = string.Empty;

        //Номер документа ИРД
        public int NumberType { get; set; }

        //Может быть единственным в комплекте или нет
        public bool IsOnlyTypeDocInRepairLog { get; set; }

        public List<Document> Documents { get; set; }
    }
}
