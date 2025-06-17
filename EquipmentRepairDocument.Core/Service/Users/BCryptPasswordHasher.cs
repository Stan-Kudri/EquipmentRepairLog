namespace EquipmentRepairDocument.Core.Service.Users
{
    /// <summary>
    /// BCrypt Password Hasher.
    /// </summary>
    public sealed class BCryptPasswordHasher : IPasswordHasher
    {
        /// <inheritdoc/>
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }
    }
}
