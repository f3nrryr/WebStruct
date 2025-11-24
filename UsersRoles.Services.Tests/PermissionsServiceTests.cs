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

namespace UsersRoles.Services.Tests
{
    [TestFixture]
    public class PermissionsServiceTests
    {
        private Mock<IPermissionsRepository> _mockPermissionsRepository;
        private PermissionsService _permissionsService;

        [SetUp]
        public void Setup()
        {
            _mockPermissionsRepository = new Mock<IPermissionsRepository>();
            _permissionsService = new PermissionsService(_mockPermissionsRepository.Object);
        }

        [Test]
        public async Task CreatePermissionAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var permissionDto = new PermissionCreateServiceModel
            {
                Name = "TestPermission",
                Description = "Test Description",
                CreatedBy = Guid.NewGuid()
            };

            var createdPermission = new PermissionModel
            {
                Id = 1,
                Name = permissionDto.Name,
                Description = permissionDto.Description,
                CreatedBy = permissionDto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            _mockPermissionsRepository.Setup(x => x.GetByNameAsync(permissionDto.Name))
                .ReturnsAsync((PermissionModel)null);
            _mockPermissionsRepository.Setup(x => x.CreateAsync(It.IsAny<PermissionCreateModel>()))
                .ReturnsAsync(createdPermission);

            // Act
            var result = await _permissionsService.CreatePermissionAsync(permissionDto);

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(createdPermission.Id, result.Data.Id);
            Assert.AreEqual(createdPermission.Name, result.Data.Name);
        }

        [Test]
        public async Task CreatePermissionAsync_EmptyName_ReturnsFailure()
        {
            // Arrange
            var permissionDto = new PermissionCreateServiceModel
            {
                Name = "",
                Description = "Test Description",
                CreatedBy = Guid.NewGuid()
            };

            // Act
            var result = await _permissionsService.CreatePermissionAsync(permissionDto);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Permission name is required", result.Errors.First());
        }

        [Test]
        public async Task CreatePermissionAsync_ExistingName_ReturnsFailure()
        {
            // Arrange
            var permissionDto = new PermissionCreateServiceModel
            {
                Name = "ExistingPermission",
                Description = "Test Description",
                CreatedBy = Guid.NewGuid()
            };

            var existingPermission = new PermissionModel { Id = 1, Name = permissionDto.Name };

            _mockPermissionsRepository.Setup(x => x.GetByNameAsync(permissionDto.Name))
                .ReturnsAsync(existingPermission);

            // Act
            var result = await _permissionsService.CreatePermissionAsync(permissionDto);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Permission with this name already exists", result.Errors.First());
        }

        [Test]
        public async Task DeletePermissionAsync_ValidId_ReturnsSuccess()
        {
            // Arrange
            var permissionId = 1;
            var permission = new PermissionModel { Id = permissionId, Name = "TestPermission" };

            _mockPermissionsRepository.Setup(x => x.GetByIdAsync(permissionId))
                .ReturnsAsync(permission);
            _mockPermissionsRepository.Setup(x => x.GetRolesWithPermissionAsync(permissionId))
                .ReturnsAsync(new List<RoleModel>());
            _mockPermissionsRepository.Setup(x => x.DeleteAsync(permissionId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _permissionsService.DeletePermissionAsync(permissionId);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task DeletePermissionAsync_PermissionNotFound_ReturnsFailure()
        {
            // Arrange
            var permissionId = 999;

            _mockPermissionsRepository.Setup(x => x.GetByIdAsync(permissionId))
                .ReturnsAsync((PermissionModel)null);

            // Act
            var result = await _permissionsService.DeletePermissionAsync(permissionId);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Permission not found", result.Errors.First());
        }

        [Test]
        public async Task AddPermissionToRoleAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var roleId = "role1";
            var permissionId = 1;
            var grantedBy = Guid.NewGuid();
            var permission = new PermissionModel { Id = permissionId, Name = "TestPermission" };

            _mockPermissionsRepository.Setup(x => x.GetByIdAsync(permissionId))
                .ReturnsAsync(permission);
            _mockPermissionsRepository.Setup(x => x.AddPermissionToRoleAsync(roleId, permissionId, grantedBy))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _permissionsService.AddPermissionToRoleAsync(roleId, permissionId, grantedBy);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task AddPermissionToRoleAsync_PermissionNotFound_ReturnsFailure()
        {
            // Arrange
            var roleId = "role1";
            var permissionId = 999;
            var grantedBy = Guid.NewGuid();

            _mockPermissionsRepository.Setup(x => x.GetByIdAsync(permissionId))
                .ReturnsAsync((PermissionModel)null);

            // Act
            var result = await _permissionsService.AddPermissionToRoleAsync(roleId, permissionId, grantedBy);

            // Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Permission not found", result.Errors.First());
        }

        [Test]
        public async Task GetPermissionByIdAsync_ValidId_ReturnsPermission()
        {
            // Arrange
            var permissionId = 1;
            var permission = new PermissionModel 
            { 
                Id = permissionId, 
                Name = "TestPermission",
                Description = "Test Description"
            };

            _mockPermissionsRepository.Setup(x => x.GetByIdAsync(permissionId))
                .ReturnsAsync(permission);

            // Act
            var result = await _permissionsService.GetPermissionByIdAsync(permissionId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(permission.Id, result.Id);
            Assert.AreEqual(permission.Name, result.Name);
        }

        [Test]
        public async Task GetAllPermissionsAsync_ReturnsAllPermissions()
        {
            // Arrange
            var permissions = new List<PermissionModel>
            {
                new PermissionModel { Id = 1, Name = "Permission1" },
                new PermissionModel { Id = 2, Name = "Permission2" }
            };

            _mockPermissionsRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(permissions);

            // Act
            var result = await _permissionsService.GetAllPermissionsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task SetRolePermissionsAsync_ValidData_ReturnsSuccess()
        {
            // Arrange
            var roleId = "role1";
            var permissionIds = new List<int> { 1, 2 };
            var grantedBy = Guid.NewGuid();

            _mockPermissionsRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new PermissionModel { Id = 1, Name = "Permission1" });
            _mockPermissionsRepository.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new PermissionModel { Id = 2, Name = "Permission2" });
            _mockPermissionsRepository.Setup(x => x.SetRolePermissionsAsync(roleId, permissionIds, grantedBy))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _permissionsService.SetRolePermissionsAsync(roleId, permissionIds, grantedBy);

            // Assert
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public async Task UserHasPermissionAsync_UserHasPermission_ReturnsTrue()
        {
            // Arrange
            var userId = "user1";
            var permissionId = 1;

            _mockPermissionsRepository.Setup(x => x.UserHasPermissionAsync(userId, permissionId))
                .ReturnsAsync(true);

            // Act
            var result = await _permissionsService.UserHasPermissionAsync(userId, permissionId);

            // Assert
            Assert.IsTrue(result);
        }
    }
}