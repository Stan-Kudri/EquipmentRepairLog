using EquipmentRepairDocument.Core.Data.Users;
using EquipmentRepairDocument.Core.DBContext;
using EquipmentRepairDocument.Core.Exceptions;
using EquipmentRepairDocument.Core.Service.Users;
using FluentAssertions;
using NSubstitute;

namespace EquipmentRepairDocument.Test.ServiceTest
{
    public class UserServiceTest
    {
        public static IEnumerable<object[]> AddUser =>
        [
            [
                "SergeySA",
                "Qwe123456",
            ]
        ];

        public static IEnumerable<object[]> AddUserInvalidPassword =>
        [
            [
                "SergeySA",
                "123",
            ]
        ];

        public static IEnumerable<object[]> AddUserInvalidUsername =>
        [
            [
                "Ser",
                "Qwe123456",
            ]
        ];

        [Fact(DisplayName = "Should throw exception when username format is invalid")]
        public async Task AddAsync_Invalid_Username_Format_Throws_Exception()
        {
            const string invalidMessage = "Invalid username passed.";
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userValidator = Substitute.For<IUserValidator>();
            userValidator.ValidateUsername(Arg.Any<string>(), out _)
                .Returns(x =>
                {
                    x[1] = invalidMessage;
                    return false;
                });
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var service = new UserService(dbContext, userValidator, passwordHasher);

            var error = await Assert.ThrowsAnyAsync<BusinessLogicException>(() => service.AddAsync("username", "password"));

            error.Message.Should().Be(invalidMessage);
        }

        [Fact(DisplayName = "Should throw exception when password format is invalid")]
        public async Task AddAsync_Invalid_Password_Format_Throws_Exception()
        {
            const string invalidMessage = "Invalid username passed.";
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userValidator = Substitute.For<IUserValidator>();
            userValidator.ValidateUsername(Arg.Any<string>(), out _).Returns(true);
            userValidator.ValidatePassword(Arg.Any<string>(), out _)
                .Returns(x =>
                {
                    x[1] = invalidMessage;
                    return false;
                });
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var service = new UserService(dbContext, userValidator, passwordHasher);

            var error = await Assert.ThrowsAnyAsync<BusinessLogicException>(() => service.AddAsync("username", "password"));

            error.Message.Should().Be(invalidMessage);
        }

        [Fact(DisplayName = "Should throw exception when username is null")]
        public async Task AddAsync_Username_Is_Null_Throws_Exception()
        {
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userValidator = Substitute.For<IUserValidator>();
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var service = new UserService(dbContext, userValidator, passwordHasher);

            await FluentActions.Invoking(() => service.AddAsync(null!, "password"))
                               .Should().ThrowAsync<BusinessLogicException>();
        }

        [Fact(DisplayName = "Should throw exception when password is null")]
        public async Task AddAsync_Password_Is_Null_Throws_Exception()
        {
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userValidator = Substitute.For<IUserValidator>();
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var service = new UserService(dbContext, userValidator, passwordHasher);

            await FluentActions.Invoking(() => service.AddAsync("username", null!))
                               .Should().ThrowAsync<BusinessLogicException>();
        }

        [Fact(DisplayName = "Should return false when username already exists")]
        public async Task IsFreeUsername_Existing_Username_Returns_False_Async()
        {
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            dbContext.Users.Add(new User("existingUser", "hash"));
            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            var userValidator = Substitute.For<IUserValidator>();
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var service = new UserService(dbContext, userValidator, passwordHasher);

            var result = service.IsFreeUsername("existingUser");
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "Should return true when username does not exist")]
        public async Task IsFreeUsername_Non_Existing_Username_Returns_True_Async()
        {
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userValidator = Substitute.For<IUserValidator>();
            var passwordHasher = Substitute.For<IPasswordHasher>();
            var service = new UserService(dbContext, userValidator, passwordHasher);

            var result = service.IsFreeUsername("newUser");
            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(AddUser))]
        public async Task Service_Should_Add_User_Of_Database(string userName, string userPassword)
        {
            //Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userService = new UserService(dbContext, new UserValidator(), new BCryptPasswordHasher());
            await userService.AddAsync(userName, userPassword);
            var expectUser = userService.GetUser(userName, userPassword);

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
            var userService = new UserService(dbContext, new UserValidator(), new BCryptPasswordHasher());

            //Assert
            await Assert.ThrowsAsync<BusinessLogicException>(async () => await userService.AddAsync(userName, userPassword));
        }

        [Theory]
        [MemberData(nameof(AddUser))]
        public async Task Did_Not_Happen_Add_Items_Because_Username_Exists(string userName, string userPassword)
        {
            //Arrange
            using var dbContext = await TestDBContextFactory.Create<AppDbContext>();
            var userService = new UserService(dbContext, new UserValidator(), new BCryptPasswordHasher());
            await userService.AddAsync(userName, userPassword);

            //Assert
            await Assert.ThrowsAsync<BusinessLogicException>(async () => await userService.AddAsync(userName, userPassword));
        }
    }
}
