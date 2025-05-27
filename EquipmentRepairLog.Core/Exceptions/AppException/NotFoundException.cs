namespace EquipmentRepairLog.Core.Exceptions.AppException
{
    public class NotFoundException : BusinessLogicException
    {
        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
