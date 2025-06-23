namespace EquipmentRepairDocument.Core.Service.Users
{
    /// <summary>
    /// Password hasher.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hash password.
        /// </summary>
        /// <param name="password">Password.</param>
        /// <returns>Hash.</returns>
        string Hash(string password);
    }
}
