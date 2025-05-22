using System.Diagnostics.CodeAnalysis;

namespace EquipmentRepairLog.Core.Data.Users
{
    public class User : Entity
    {
        private User()
        {
        }

        [SetsRequiredMembers]
        public User(string username, string passwordHash)
        {
            ArgumentException.ThrowIfNullOrEmpty(username);
            ArgumentException.ThrowIfNullOrEmpty(passwordHash);

            Username = username;
            PasswordHash = passwordHash;
        }

        [SetsRequiredMembers]
        public User(string username, string passwordHash, UserRole userRole)
            : this(username, passwordHash) => UserRole = userRole;

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public required string PasswordHash { get; set; }

        /// <summary>
        /// Роль пользователя в прилажении
        /// </summary>
        public UserRole UserRole { get; set; } = UserRole.Viewer;
    }
}
