using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersRoles.Repositories.Interfaces;
using UsersRoles.Repositories.DTOs;
using UsersRoles.Services.Implementations;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Enums;

namespace UsersRoles.Services.Tests
{
    [TestFixture]
    public class UsersServiceTests
    {
        private Mock<IUsersRepository> _mockUsersRepository;
        private Mock<IRolesRepository> _mockRolesRepository;
        private Mock<IPermissionsRepository> _mockPermissionsRepository;
        private UsersService _usersService;

        [SetUp]
        public void Setup()
        {
            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockRolesRepository = new Mock<IRolesRepository>();
            _mockPermissionsRepository = new Mock<IPermissionsRepository>();
            
            _usersService = new UsersService(
                _mockUsersRepository.Object,
                _mockRolesRepository.Object,
                _mockPermissionsRepository.Object);
        }

        [Test]
        public async Task CreateUserAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var userDto = new UserCreateServiceModel
            {
                UserName = "testuser",
                Email = "test@example.com",
                FullName = "Test User",
                Password = "Password123!",
                CreatedBy = Guid.NewGuid(),
                Roles = new List<string> { "User" }
            };

            var createdUser = new UserModel
            {
                Id = "user1",
                UserName = userDto.UserName,
                Email = userDto.Email,
                FullName = userDto.FullName,
                CreatedBy = userDto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                EmailConfirmed = false
            };

            _mockUsersRepository.Setup(x => x.GetByEmailAsync(userDto.Email))
                .ReturnsAsync((UserModel)null);
            _mockUsersRepository.Setup(x => x.CreateAsync(It.IsAny<UserCreateModel>(), userDto.Password))
                .ReturnsAsync(createdUser);
            _mockRolesRepository.Setup(x => x.ExistsAsync("User"))
                .ReturnsAsync(true);
            _mockUsersRepository.Setup(x => x.AddToRoleAsync(createdUser.Id, "User"))
                .Returns(Task.CompletedTask);

            _mockUsersRepository.Setup(x => x.GetByIdAsync(createdUser.Id))
                .ReturnsAsync(createdUser);
            _mockUsersRepository.Setup(x => x.GetUserRolesAsync(createdUser.Id))
                .ReturnsAsync(new List<string> { "User" });
            _mockPermissionsRepository.Setup(x => x.GetUserPermissionsAsync(createdUser.Id))
                .ReturnsAsync(new List<PermissionModel>());

            // Act
            var result = await _usersService.CreateUserAsync(userDto);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(createdUser.Id, result.Data.Id);
            Assert.AreEqual(createdUser.Email, result.Data.Email);
        }

        [Test]
        public async Task CreateUserAsync_EmptyEmail_ReturnsFailure()
        {
            // Arrange
            var userDto = new UserCreateServiceModel
            {
                Email = "",
                Password = "Password123!"
            };

            // Act
            var result = await _usersService.CreateUserAsync(userDto);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Email is required", result.Errors.First());
        }

        [Test]
        public async Task CreateUserAsync_ExistingEmail_ReturnsFailure()
        {
            // Arrange
            var userDto = new UserCreateServiceModel
            {
                Email = "existing@example.com",
                Password = "Password123!"
            };

            var existingUser = new UserModel { Id = "user1", Email = userDto.Email };

            _mockUsersRepository.Setup(x => x.GetByEmailAsync(userDto.Email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _usersService.CreateUserAsync(userDto);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("User with this email already exists", result.Errors.First());
        }

        [Test]
        public async Task ChangePasswordAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var userId = "user1";
            var currentPassword = "OldPassword123!";
            var newPassword = "NewPassword123!";
            var user = new UserModel { Id = userId, UserName = "testuser" };

            _mockUsersRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUsersRepository.Setup(x => x.CheckPasswordAsync(userId, currentPassword))
                .ReturnsAsync(true);
            _mockUsersRepository.Setup(x => x.ChangePasswordAsync(userId, currentPassword, newPassword))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _usersService.ChangePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task ChangePasswordAsync_InvalidCurrentPassword_ReturnsFailure()
        {
            // Arrange
            var userId = "user1";
            var currentPassword = "WrongPassword";
            var newPassword = "NewPassword123!";
            var user = new UserModel { Id = userId, UserName = "testuser" };

            _mockUsersRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUsersRepository.Setup(x => x.CheckPasswordAsync(userId, currentPassword))
                .ReturnsAsync(false);

            // Act
            var result = await _usersService.ChangePasswordAsync(userId, currentPassword, newPassword);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Current password is incorrect", result.Errors.First());
        }

        [Test]
        public async Task AssignRolesToUserAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var userId = "user1";
            var roles = new List<string> { "Admin", "User" };
            var user = new UserModel { Id = userId, UserName = "testuser" };

            _mockUsersRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockRolesRepository.Setup(x => x.ExistsAsync("Admin"))
                .ReturnsAsync(true);
            _mockRolesRepository.Setup(x => x.ExistsAsync("User"))
                .ReturnsAsync(true);
            _mockUsersRepository.Setup(x => x.SetUserRolesAsync(userId, roles))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _usersService.AssignRolesToUserAsync(userId, roles);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task GetUserByIdAsync_ValidId_ReturnsUserWithRoles()
        {
            // Arrange
            var userId = "user1";
            var user = new UserModel 
            { 
                Id = userId, 
                UserName = "testuser",
                Email = "test@example.com",
                FullName = "Test User"
            };

            var roles = new List<string> { "Admin", "User" };
            var permissions = new List<PermissionModel>
            {
                new PermissionModel { Id = 1, Name = "Users.Read" }
            };

            _mockUsersRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUsersRepository.Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync(roles);
            _mockPermissionsRepository.Setup(x => x.GetUserPermissionsAsync(userId))
                .ReturnsAsync(permissions);

            // Act
            var result = await _usersService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.Email, result.Email);
        }

        [Test]
        public async Task UserHasPermissionAsync_UserHasPermission_ReturnsTrue()
        {
            // Arrange
            var userId = "user1";
            var permission = PermissionEnum.UsersRead;

            _mockPermissionsRepository.Setup(x => x.UserHasPermissionAsync(userId, "Users.Read"))
                .ReturnsAsync(true);

            // Act
            var result = await _usersService.UserHasPermissionAsync(userId, permission);

            // Assert
            Assert.IsTrue(result);
        }
    }
}