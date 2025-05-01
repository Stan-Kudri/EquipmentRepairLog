using EquipmentRepairLog.Core.Data.User;
using EquipmentRepairLog.Core.DBContext;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRepairLog.Core.Service
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext appDbContext) => _dbContext = appDbContext;

        public void Add(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (_dbContext.Users.Any(e => e.Username == user.Username))
            {
                throw new ArgumentException("This username exists.");
            }

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public User? GetUser(string username, string passwordHash)
            => _dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username && e.PasswordHash == passwordHash);

        public bool IsFreeUsername(string username)
            => _dbContext.Users.AsNoTracking().FirstOrDefault(e => e.Username == username) == null;
    }
}
