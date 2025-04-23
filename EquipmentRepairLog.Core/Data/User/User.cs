using System.Security.Cryptography;
using System.Text;

namespace EquipmentRepairLog.Core.Data.User
{
    public class User : Entity
    {
        private User()
        {
        }

        public User(string username, string password)
        {
            var validator = new UserValidator();

            if (!validator.ValidFormatPassword(password, out var messageValidPass))
            {
                throw new ArgumentException(messageValidPass, nameof(password));
            }

            if (!validator.ValidFormatUsername(username, out var messageValidUsername))
            {
                throw new ArgumentException(messageValidUsername, nameof(username));
            }

            Username = username;
            PasswordHash = GetPaswordHash(password);
        }

        public User(string username, string passwordHash, UserRole userRole)
            : this(username, passwordHash) => UserRole = userRole;

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

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
