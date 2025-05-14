namespace EquipmentRepairLog.Core.Exceptions
{
    public class BusinessLogicException : ApplicationException
    {
        public BusinessLogicException(string message)
            : base(message)
        {
        }

        public BusinessLogicException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static BusinessLogicException InvalidFormat<T>(string property, byte minLenght, byte maxLenght)
            => new BusinessLogicException(($"The {property} {typeof(T).Name} is out of range [{minLenght}...{maxLenght}]."));
    }
}
