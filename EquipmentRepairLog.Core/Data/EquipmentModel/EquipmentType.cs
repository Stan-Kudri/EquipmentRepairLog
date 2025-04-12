namespace EquipmentRepairLog.Core.Data.EquipmentModel
{
    public class EquipmentType : Entity
    {
        /// <summary>
        /// Наименование типа и(или) марки
        /// </summary>
        public required string Name { get; set; }
    }
}
