namespace EquipmentRepairDocument.Core.Service.Users
{
    public interface IUserValidator
    {
        bool ValidFormatPassword(string password, out string message);

        bool ValidFormatUsername(string username, out string message);
    }
}
