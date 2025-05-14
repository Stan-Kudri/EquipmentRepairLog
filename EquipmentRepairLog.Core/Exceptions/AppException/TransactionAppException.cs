namespace EquipmentRepairLog.Core.Exceptions.AppException
{
    public class TransactionAppException : BusinessLogicException
    {
        public TransactionAppException(string message) : base(message)
        {
        }
        public TransactionAppException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
