using EquipmentRepairLog.Core.Data.Users;
using EquipmentRepairLog.Core.DBContext;
using EquipmentRepairLog.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class UserService(AppDbContext dbContext, UserValidator userValidator)
    {
        public async Task AddAsync(string username, string password)
        {
            BusinessLogicException.ThrowIfNull(username);
            BusinessLogicException.ThrowIfNull(password);

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

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }

        public User? GetUser(string username, string passwordHash)
            => dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username && e.PasswordHash == passwordHash);

        public bool IsFreeUsername(string username)
            => dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username) == null;

        private string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}
