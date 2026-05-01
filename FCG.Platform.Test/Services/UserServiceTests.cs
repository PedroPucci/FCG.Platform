using FCG.Platform.Application.Services;
using FCG.Platform.Domain.Entities.Dto.UserDto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using FCG.Platform.Shared.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FCG.Platform.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IRepositoryUoW> _repositoryUoWMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly Mock<UserManager<UserEntity>> _userManagerMock;
        private readonly Mock<RoleManager<ProfileEntity>> _roleManagerMock;

        private readonly UserService _service;

        public UserServiceTests()
        {
            _repositoryUoWMock = new Mock<IRepositoryUoW>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _transactionMock = new Mock<IDbContextTransaction>();

            _repositoryUoWMock
                .Setup(x => x.UserRepository)
                .Returns(_userRepositoryMock.Object);

            _repositoryUoWMock
                .Setup(x => x.BeginTransaction())
                .Returns(_transactionMock.Object);

            _transactionMock
                .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock
                .Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();

            _service = new UserService(
                _repositoryUoWMock.Object,
                _userManagerMock.Object,
                _roleManagerMock.Object);
        }

        private static Mock<UserManager<UserEntity>> MockUserManager()
        {
            var store = new Mock<IUserStore<UserEntity>>();

            return new Mock<UserManager<UserEntity>>(
                store.Object,
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<UserEntity>>(),
                Array.Empty<IUserValidator<UserEntity>>(),
                Array.Empty<IPasswordValidator<UserEntity>>(),
                Mock.Of<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<UserEntity>>>());
        }

        private static Mock<RoleManager<ProfileEntity>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<ProfileEntity>>();

            return new Mock<RoleManager<ProfileEntity>>(
                store.Object,
                Array.Empty<IRoleValidator<ProfileEntity>>(),
                Mock.Of<ILookupNormalizer>(),
                new IdentityErrorDescriber(),
                Mock.Of<ILogger<RoleManager<ProfileEntity>>>());
        }

        [Fact]
        public async Task Should_Add_User_Successfully_When_Request_Is_Valid()
        {
            var request = new UserResponse
            {
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123",
                Role = "Administrator"
            };

            _roleManagerMock
                .Setup(x => x.RoleExistsAsync(request.Role))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), request.Role))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _service.Add(request);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(request.Name, result.Data.Name);
            Assert.Equal(request.Email, result.Data.Email);
            Assert.Equal(request.Email, result.Data.UserName);
            Assert.True(result.Data.IsActive);

            _roleManagerMock.Verify(x => x.RoleExistsAsync(request.Role), Times.Once);

            _userManagerMock.Verify(x => x.CreateAsync(
                It.Is<UserEntity>(u =>
                    u.Name == request.Name &&
                    u.Email == request.Email &&
                    u.UserName == request.Email &&
                    u.IsActive),
                request.Password), Times.Once);

            _userManagerMock.Verify(x => x.AddToRoleAsync(
                It.IsAny<UserEntity>(),
                request.Role), Times.Once);

            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Error_When_User_Request_Is_Invalid()
        {
            var request = new UserResponse
            {
                Name = "Pedro",
                Email = "",
                Password = "123",
                Role = "Administrator"
            };

            var result = await _service.Add(request);

            Assert.False(result.Success);

            _roleManagerMock.Verify(x => x.RoleExistsAsync(It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Error_When_Role_Is_Empty()
        {
            var request = new UserResponse
            {
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123",
                Role = ""
            };

            var result = await _service.Add(request);

            Assert.False(result.Success);
            Assert.Equal("'Role' can not be null or empty!", result.Message);

            _roleManagerMock.Verify(x => x.RoleExistsAsync(It.IsAny<string>()), Times.Never);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Error_When_Role_Does_Not_Exist()
        {
            var request = new UserResponse
            {
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123",
                Role = "InvalidRole"
            };

            _roleManagerMock
                .Setup(x => x.RoleExistsAsync(request.Role))
                .ReturnsAsync(false);

            var result = await _service.Add(request);

            Assert.False(result.Success);
            Assert.Equal("Invalid role. Use only: Administrator or Usuario.", result.Message);

            _roleManagerMock.Verify(x => x.RoleExistsAsync(request.Role), Times.Once);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Error_When_CreateAsync_Fails()
        {
            var request = new UserResponse
            {
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123",
                Role = "Administrator"
            };

            _roleManagerMock
                .Setup(x => x.RoleExistsAsync(request.Role))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError
                {
                    Description = "Error creating user."
                }));

            var result = await _service.Add(request);

            Assert.False(result.Success);
            Assert.Contains("Error creating user.", result.Message);

            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Error_When_AddToRoleAsync_Fails()
        {
            var request = new UserResponse
            {
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123",
                Role = "Administrator"
            };

            _roleManagerMock
                .Setup(x => x.RoleExistsAsync(request.Role))
                .ReturnsAsync(true);

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), request.Role))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError
                {
                    Description = "Error adding role."
                }));

            var result = await _service.Add(request);

            Assert.False(result.Success);
            Assert.Contains("Error adding role.", result.Message);

            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Rollback_And_Throw_Exception_When_Exception_Occurs_On_Add()
        {
            var request = new UserResponse
            {
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123",
                Role = "Administrator"
            };

            _roleManagerMock
                .Setup(x => x.RoleExistsAsync(request.Role))
                .ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.Add(request));

            Assert.Equal("Database error", exception.Message);

            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Return_Success_When_User_Exists()
        {
            var user = new UserEntity
            {
                Id = "1",
                Name = "Pedro",
                Email = "old@email.com",
                IsActive = true
            };

            var updateUserRequest = new UpdateUserRequest
            {
                Name = "Novo Nome",
                Email = "novo@email.com",
                IsActive = false
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(user.Id))
                .ReturnsAsync(user);

            _repositoryUoWMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.Update(user.Id, updateUserRequest);

            Assert.True(result.Success);
            Assert.True(result.Data);

            _userRepositoryMock.Verify(x => x.Update(It.Is<UserEntity>(u =>
                u.Id == user.Id &&
                u.Name == updateUserRequest.Name &&
                u.Email == updateUserRequest.Email &&
                u.IsActive == updateUserRequest.IsActive)), Times.Once);

            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Return_Error_When_User_Not_Found()
        {
            var userId = "1";

            var updateUserRequest = new UpdateUserRequest
            {
                Name = "Pedro",
                Email = "email@email.com",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync((UserEntity?)null);

            var result = await _service.Update(userId, updateUserRequest);

            Assert.False(result.Success);
            Assert.Equal(LogMessages.CannotPerformActionOnUser("update", userId), result.Message);

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<UserEntity>()), Times.Never);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Delete_Should_Return_Success_When_User_Exists()
        {
            var userId = "1";

            var user = new UserEntity
            {
                Id = userId,
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync(user);

            _repositoryUoWMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.Delete(userId);

            Assert.True(result.Success);
            Assert.False(user.IsActive);

            _userRepositoryMock.Verify(x => x.Update(It.Is<UserEntity>(u =>
                u.Id == userId &&
                u.IsActive == false)), Times.Once);

            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task Delete_Should_Return_Error_When_User_Not_Found()
        {
            var userId = "1";

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync((UserEntity?)null);

            var result = await _service.Delete(userId);

            Assert.False(result.Success);
            Assert.Equal(LogMessages.CannotPerformActionOnUser("retrieve", userId), result.Message);

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<UserEntity>()), Times.Never);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transactionMock.Verify(x => x.Rollback(), Times.Once);
        }

        [Fact]
        public async Task Get_Should_Return_User_List_When_Successful()
        {
            var users = new List<UserResponse>
            {
                new UserResponse
                {
                    Name = "Pedro",
                    Email = "pedro@email.com",
                    IsActive = true
                },
                new UserResponse
                {
                    Name = "Maria",
                    Email = "maria@email.com",
                    IsActive = true
                }
            };

            _userRepositoryMock
                .Setup(x => x.Get())
                .ReturnsAsync(users);

            var result = await _service.Get();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Pedro", result[0].Name);
            Assert.Equal("Maria", result[1].Name);
            Assert.True(result[0].IsActive);
            Assert.True(result[1].IsActive);

            _userRepositoryMock.Verify(x => x.Get(), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Once);
            _transactionMock.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task Get_Should_Throw_Exception_When_Repository_Fails()
        {
            _userRepositoryMock
                .Setup(x => x.Get())
                .ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.Get());

            Assert.Equal("Error to loading the list User. See logs for details.", exception.Message);

            _userRepositoryMock.Verify(x => x.Get(), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Never);
            _transactionMock.Verify(x => x.Rollback(), Times.Once);
        }

        [Fact]
        public async Task GetById_Should_Return_User_When_User_Exists()
        {
            var userId = "1";

            var user = new UserEntity
            {
                Id = userId,
                Name = "Pedro",
                Email = "PEDRO@EMAIL.COM",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync(user);

            var result = await _service.GetById(userId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Pedro", result.Data.Name);
            Assert.Equal("PEDRO@EMAIL.COM", result.Data.Email);
            Assert.True(result.Data.IsActive);

            _userRepositoryMock.Verify(x => x.GetByIdCheck(userId), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Once);
            _transactionMock.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task GetById_Should_Return_Error_When_User_Does_Not_Exist()
        {
            var userId = "1";

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync((UserEntity?)null);

            var result = await _service.GetById(userId);

            Assert.False(result.Success);
            Assert.Equal(LogMessages.CannotPerformActionOnUser("retrieve", userId), result.Message);

            _userRepositoryMock.Verify(x => x.GetByIdCheck(userId), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Never);
            _transactionMock.Verify(x => x.Rollback(), Times.Once);
        }
    }
}