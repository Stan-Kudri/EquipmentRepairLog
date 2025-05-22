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

        public static void EnsureRange<T>(byte value, string propertyName, byte minLenght, byte maxLenght)
        {
            if (value <= maxLenght && value >= minLenght)
            {
                return;
            }

            InvalidFormat<T>(propertyName, minLenght, maxLenght);
        }

        public static void EnsureLength<T>(string property, string propertyName, byte minLenght, byte maxLenght)
        {
            if (property.Length <= maxLenght && property.Length >= minLenght)
            {
                return;
            }

            InvalidFormat<T>(propertyName, minLenght, maxLenght);
        }
    }
}
