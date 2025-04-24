using EquipmentRepairLog.Core.Data.User;
using EquipmentRepairLog.Core.DBContext;

namespace EquipmentRepairLog.Core.Service
{
    public class UserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext appDbContext) => _dbContext = appDbContext;

        public void Add(User user)
        {
            if (user == null)
            {
                throw new ArgumentException("The received parameters are not correct.");
            }

            if (_dbContext.Users.Any(e => e.Username == user.Username))
            {
                throw new ArgumentException("This username exists.");
            }

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public User? GetUser(string username, string passwordHash)
            => _dbContext.Users.FirstOrDefault(e => e.Username == username && e.PasswordHash == passwordHash);

        public bool IsFreeUsername(string username)
            => _dbContext.Users.FirstOrDefault(e => e.Username == username) == null;

        public bool IsUserData(User user)
            => _dbContext.Users.FirstOrDefault(e => e.Username == user.Username && e.PasswordHash == user.PasswordHash) != null;
    }
}
