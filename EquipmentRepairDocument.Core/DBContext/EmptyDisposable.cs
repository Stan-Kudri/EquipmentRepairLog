namespace EquipmentRepairDocument.Core.DBContext
{
    public class EmptyDisposable : IDisposable
    {
        public static readonly IDisposable Instance = new EmptyDisposable();

        private EmptyDisposable()
        {
        }

        public void Dispose()
        {
        }
    }
}
