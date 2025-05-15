using System.Security.Cryptography;
using System.Text;

namespace EquipmentRepairLog.Core.Data.Users
{
    public class User : Entity
    {
        private static readonly UserValidator userValidator = new UserValidator();

        private User()
        {
        }

        public User(string username, string password)
        {
            ArgumentException.ThrowIfNullOrEmpty(username);
            ArgumentException.ThrowIfNullOrEmpty(password);

            if (!userValidator.ValidFormatPassword(password, out var messageValidPass))
            {
                throw new ArgumentException(messageValidPass, nameof(password));
            }

            if (!userValidator.ValidFormatUsername(username, out var messageValidUsername))
            {
                throw new ArgumentException(messageValidUsername, nameof(username));
            }

            Username = username;
            Password = password;
            PasswordHash = GetPaswordHash(Password);
        }

        public User(string username, string passwordHash, UserRole userRole)
            : this(username, passwordHash) => UserRole = userRole;

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public required string Username
        {
            get => Username;
            set
            {
                if (!userValidator.ValidFormatUsername(Username, out var messageValidUsername))
                {
                    throw new ArgumentException(messageValidUsername, nameof(Username));
                }

                Username = value;
            }
        }

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public required string PasswordHash
        {
            get => PasswordHash;
            set
            {
                if (!userValidator.ValidFormatPassword(value, out var messageValidPass))
                {
                    throw new ArgumentException(messageValidPass, nameof(value));
                }

                PasswordHash = GetPaswordHash(value);
            }
        }

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public string Password
        {
            get => Password;
            set
            {
                if (!userValidator.ValidFormatPassword(value, out var messageValidPass))
                {
                    throw new ArgumentException(messageValidPass, nameof(value));
                }

                Password = value;
            }
        }

        /// <summary>
        /// Роль пользователя в прилажении
        /// </summary>
        public UserRole UserRole { get; set; } = UserRole.Viewer;

        private string GetPaswordHash(string password)
        {
            MD5 MD5Hash = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            byte[] hash = MD5Hash.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
