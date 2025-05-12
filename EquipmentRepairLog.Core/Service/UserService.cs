using EquipmentRepairLog.Core.Data.User;
using EquipmentRepairLog.Core.DBContext;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class UserService(AppDbContext dbContext)
    {
        public void Add(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (dbContext.Users.Any(e => e.Username == user.Username))
            {
                throw new ArgumentException($"This username {user.Username} exists.");
            }

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        public User? GetUser(string username, string passwordHash)
            => dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username && e.PasswordHash == passwordHash);

        public bool IsFreeUsername(string username)
            => dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username) == null;
    }
}
