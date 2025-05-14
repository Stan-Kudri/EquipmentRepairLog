namespace EquipmentRepairLog.Core.Exceptions
{
    public class EquipmentRepairLogException : ApplicationException
    {
        public EquipmentRepairLogException(string message)
            : base(message)
        {
        }

        public EquipmentRepairLogException(object value, Type type, byte maxLenght, byte minLenght)
            : this($"{nameof(value)} {type.Name} is out of range [{minLenght}...{maxLenght}].")
        {
        }
    }
}
