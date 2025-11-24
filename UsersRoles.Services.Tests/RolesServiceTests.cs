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
    public class RolesServiceTests
    {
        private Mock<IRolesRepository> _mockRolesRepository;
        private Mock<IPermissionsRepository> _mockPermissionsRepository;
        private Mock<IUsersRepository> _mockUsersRepository;
        private RolesService _rolesService;

        [SetUp]
        public void Setup()
        {
            _mockRolesRepository = new Mock<IRolesRepository>();
            _mockPermissionsRepository = new Mock<IPermissionsRepository>();
            _mockUsersRepository = new Mock<IUsersRepository>();
            
            _rolesService = new RolesService(
                _mockRolesRepository.Object,
                _mockPermissionsRepository.Object,
                _mockUsersRepository.Object);
        }

        [Test]
        public async Task CreateRoleAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var roleDto = new RoleCreateServiceModel
            {
                Name = "TestRole",
                Description = "Test Description",
                CreatedBy = Guid.NewGuid(),
                Permissions = new List<PermissionEnum> { PermissionEnum.UsersRead, PermissionEnum.UsersCreate }
            };

            var createdRole = new RoleModel
            {
                Id = "role1",
                Name = roleDto.Name,
                Description = roleDto.Description,
                CreatedBy = roleDto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            _mockRolesRepository.Setup(x => x.ExistsAsync(roleDto.Name))
                .ReturnsAsync(false);
            _mockRolesRepository.Setup(x => x.CreateAsync(It.IsAny<RoleCreateModel>()))
                .ReturnsAsync(createdRole);

            _mockPermissionsRepository.Setup(x => x.GetByNameAsync("Users.Read"))
                .ReturnsAsync(new PermissionModel { Id = 1, Name = "Users.Read" });
            _mockPermissionsRepository.Setup(x => x.GetByNameAsync("Users.Write"))
                .ReturnsAsync(new PermissionModel { Id = 2, Name = "Users.Write" });

            _mockRolesRepository.Setup(x => x.GetByIdAsync(createdRole.Id))
                .ReturnsAsync(createdRole);
            _mockPermissionsRepository.Setup(x => x.GetRolePermissionsAsync(createdRole.Id))
                .ReturnsAsync(new List<PermissionModel>());

            // Act
            var result = await _rolesService.CreateRoleAsync(roleDto);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(createdRole.Id, result.Data.Id);
            Assert.AreEqual(createdRole.Name, result.Data.Name);
        }

        [Test]
        public async Task CreateRoleAsync_EmptyName_ReturnsFailure()
        {
            // Arrange
            var roleDto = new RoleCreateServiceModel
            {
                Name = "",
                Description = "Test Description",
                CreatedBy = Guid.NewGuid()
            };

            // Act
            var result = await _rolesService.CreateRoleAsync(roleDto);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Role name is required", result.Errors.First());
        }

        [Test]
        public async Task CreateRoleAsync_ExistingName_ReturnsFailure()
        {
            // Arrange
            var roleDto = new RoleCreateServiceModel
            {
                Name = "ExistingRole",
                Description = "Test Description",
                CreatedBy = Guid.NewGuid()
            };

            _mockRolesRepository.Setup(x => x.ExistsAsync(roleDto.Name))
                .ReturnsAsync(true);

            // Act
            var result = await _rolesService.CreateRoleAsync(roleDto);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Role with this name already exists", result.Errors.First());
        }

        [Test]
        public async Task DeleteRoleAsync_ValidRole_ReturnsSuccess()
        {
            // Arrange
            var roleId = "role1";
            var role = new RoleModel { Id = roleId, Name = "TestRole" };

            _mockRolesRepository.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(role);
            _mockUsersRepository.Setup(x => x.GetUsersInRoleAsync(role.Name))
                .ReturnsAsync(new List<UserModel>());
            _mockRolesRepository.Setup(x => x.DeleteAsync(roleId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _rolesService.DeleteRoleAsync(roleId);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task DeleteRoleAsync_RoleWithUsers_ReturnsFailure()
        {
            // Arrange
            var roleId = "role1";
            var role = new RoleModel { Id = roleId, Name = "TestRole" };
            var users = new List<UserModel> { new UserModel { Id = "user1", UserName = "testuser" } };

            _mockRolesRepository.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(role);
            _mockUsersRepository.Setup(x => x.GetUsersInRoleAsync(role.Name))
                .ReturnsAsync(users);

            // Act
            var result = await _rolesService.DeleteRoleAsync(roleId);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Cannot delete role that has users assigned", result.Errors.First());
        }

        [Test]
        public async Task AddPermissionToRoleAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var roleId = "role1";
            var permission = PermissionEnum.UsersRead;
            var grantedBy = Guid.NewGuid();
            var role = new RoleModel { Id = roleId, Name = "TestRole" };
            var permissionEntity = new PermissionModel { Id = 1, Name = "Users.Read" };

            _mockRolesRepository.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(role);
            _mockPermissionsRepository.Setup(x => x.GetByNameAsync("Users.Read"))
                .ReturnsAsync(permissionEntity);
            _mockPermissionsRepository.Setup(x => x.AddPermissionToRoleAsync(roleId, permissionEntity.Id, grantedBy))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _rolesService.AddPermissionToRoleAsync(roleId, permission, grantedBy);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task GetRoleByIdAsync_ValidId_ReturnsRoleWithPermissions()
        {
            // Arrange
            var roleId = "role1";
            var role = new RoleModel 
            { 
                Id = roleId, 
                Name = "TestRole",
                Description = "Test Description"
            };

            var permissions = new List<PermissionModel>
            {
                new PermissionModel { Id = 1, Name = "Users.Read" },
                new PermissionModel { Id = 2, Name = "Users.Write" }
            };

            _mockRolesRepository.Setup(x => x.GetByIdAsync(roleId))
                .ReturnsAsync(role);
            _mockPermissionsRepository.Setup(x => x.GetRolePermissionsAsync(roleId))
                .ReturnsAsync(permissions);

            // Act
            var result = await _rolesService.GetRoleByIdAsync(roleId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(role.Id, result.Id);
            Assert.AreEqual(role.Name, result.Name);
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
            var result = await _rolesService.AssignRolesToUserAsync(userId, roles);

            // Assert
            Assert.IsTrue(result.Succeeded);
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
            var result = await _rolesService.UserHasPermissionAsync(userId, permission);

            // Assert
            Assert.IsTrue(result);
        }
    }
}