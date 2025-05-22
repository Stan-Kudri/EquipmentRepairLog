using EquipmentRepairLog.Core.Data.Users;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class UserService(AppDbContext dbContext, UserValidator userValidator)
    {
        public void Add(string username, string password)
        {
            ArgumentNullException.ThrowIfNull(username);
            ArgumentNullException.ThrowIfNull(password);

            if (!userValidator.ValidFormatUsername(username, out var messageValidUsername))
            {
                throw new BusinessLogicException(messageValidUsername);
            }

            if (!userValidator.ValidFormatPassword(password, out var messageValidPass))
            {
                throw new BusinessLogicException(messageValidPass);
            }

            if (dbContext.Users.Any(e => e.Username == username))
            {
                throw new BusinessLogicException($"This username {username} exists.");
            }

            var passwordHash = Hash(password);
            var user = new User(username, passwordHash);

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        public User? GetUser(string username, string passwordHash)
            => dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username && e.PasswordHash == passwordHash);

        public bool IsFreeUsername(string username)
            => dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username) == null;

        private string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
