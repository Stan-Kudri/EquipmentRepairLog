using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Service;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class UserServiceTest
    {
        public static IEnumerable<object[]> AddUser =
        [
            [
                "SergeySA",
                "Qwe123456",
            ]
        ];

        public static IEnumerable<object[]> AddUserInvalidPassword =
        [
            [
                "SergeySA",
                "123",
            ]
        ];

        public static IEnumerable<object[]> AddUserInvalidUsername =
        [
            [
                "Ser",
                "Qwe123456",
            ]
        ];

        [Theory]
        [MemberData(nameof(AddUser))]
        public async Task Service_Should_Add_User_Of_Database(string userName, string userPassword)
        {
            //Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userService = new UserService(dbContext);
            await userService.AddAsync(userName, userPassword);
            var expectUser = userService.GetUserByPassword(userName, userPassword);

            //Act
            var actualUser = dbContext.Users.FirstOrDefault(e => e.Username == userName);

            //Assert
            actualUser?.Equals(expectUser);
        }

        [Theory]
        [MemberData(nameof(AddUserInvalidPassword))]
        [MemberData(nameof(AddUserInvalidUsername))]
        public async Task Did_Not_Happen_Add_Items_Because_Invalid_Format_Data(string userName, string userPassword)
        {
            //Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userService = new UserService(dbContext);

            //Assert
            await Assert.ThrowsAsync<BusinessLogicException>(async () => await userService.AddAsync(userName, userPassword));
        }

        [Theory]
        [MemberData(nameof(AddUser))]
        public async Task Did_Not_Happen_Add_Items_Because_Username_Exists(string userName, string userPassword)
        {
            //Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userService = new UserService(dbContext);
            await userService.AddAsync(userName, userPassword);

            //Assert
            await Assert.ThrowsAsync<BusinessLogicException>(async () => await userService.AddAsync(userName, userPassword));
        }
    }
}
