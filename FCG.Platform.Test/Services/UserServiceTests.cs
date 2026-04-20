using FCG.Platform.Application.Services;
using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace FCG.Platform.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IRepositoryUoW> _repositoryUoWMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;

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
        }

        [Fact]
        public async Task Should_Add_User_Successfully_When_Request_Is_Valid()
        {
            var user = new UserEntity
            {
                Id = 1,
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123"
            };

            _userRepositoryMock
                .Setup(x => x.Add(It.IsAny<UserEntity>()))
                .ReturnsAsync(user);

            _repositoryUoWMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _transactionMock
                .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Add(user);

            Assert.True(result.Success);
            Assert.True(user.IsActive);

            _userRepositoryMock.Verify(x => x.Add(It.Is<UserEntity>(u =>
                u.Name == user.Name &&
                u.Email == user.Email &&
                u.Password == user.Password &&
                u.IsActive == true)), Times.Once);

            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Return_Error_When_User_Request_Is_Invalid()
        {
            var user = new UserEntity
            {
                Id = 1,
                Name = "Pedro",
                Email = "",
                Password = "123"
            };

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Add(user);

            Assert.False(result.Success);
            _userRepositoryMock.Verify(x => x.Add(It.IsAny<UserEntity>()), Times.Never);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Rollback_And_Return_Error_When_Exception_Occurs_On_Add()
        {
            var user = new UserEntity
            {
                Id = 1,
                Name = "Pedro Ighor",
                Email = "pedro@email.com",
                Password = "Senha@123"
            };

            _userRepositoryMock
                .Setup(x => x.Add(It.IsAny<UserEntity>()))
                .ThrowsAsync(new Exception("Database error"));

            _transactionMock
                .Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Add(user);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Message);

            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Return_Success_When_User_Exists()
        {
            var user = new UserEntity
            {
                Id = 1,
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

            _transactionMock
                .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Update(user.Id, updateUserRequest);

            Assert.True(result.Success);

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
            var userId = 1;

            var updateUserRequest = new UpdateUserRequest
            {
                Name = "Pedro",
                Email = "email@email.com",
                IsActive = true
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync((UserEntity?)null);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Update(userId, updateUserRequest);

            Assert.False(result.Success);
            Assert.Equal($"Cannot update user. User with id {userId} was not found.", result.Message);

            _userRepositoryMock.Verify(x => x.Update(It.IsAny<UserEntity>()), Times.Never);
            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Delete_Should_Return_Success_When_User_Exists()
        {
            var userId = 1;

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

            _transactionMock
                .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Delete(userId);

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
            var userId = 1;

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync((UserEntity?)null);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Delete(userId);

            Assert.False(result.Success);
            Assert.Equal($"Cannot retrieve user. User with id {userId} was not found.", result.Message);

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
                },
                new UserResponse
                {
                    Name = "Maria",
                    Email = "maria@email.com"
                }
            };

            _userRepositoryMock
                .Setup(x => x.Get())
                .ReturnsAsync(users);

            _repositoryUoWMock
                .Setup(x => x.Commit());

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Get();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Pedro", result[0].Name);
            Assert.Equal("Maria", result[1].Name);

            _userRepositoryMock.Verify(x => x.Get(), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Get_Should_Throw_Exception_When_Repository_Fails()
        {
            _userRepositoryMock
                .Setup(x => x.Get())
                .ThrowsAsync(new Exception("Database error"));

            var service = new UserService(_repositoryUoWMock.Object);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.Get());

            Assert.Contains("Error to loading the list User", exception.Message);

            _userRepositoryMock.Verify(x => x.Get(), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Never);
            _transactionMock.Verify(x => x.Rollback(), Times.Once);
        }

        [Fact]
        public async Task GetById_Should_Return_User_When_User_Exists()
        {
            var userId = 1;

            var user = new UserEntity
            {
                Id = userId,
                Name = "Pedro",
                Email = "PEDRO@EMAIL.COM"
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync(user);

            _repositoryUoWMock
                .Setup(x => x.Commit());

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.GetById(userId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Pedro", result.Data.Name);
            Assert.Equal("PEDRO@EMAIL.COM", result.Data.Email);

            _userRepositoryMock.Verify(x => x.GetByIdCheck(userId), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Once);
            _transactionMock.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task GetById_Should_Return_Error_When_User_Does_Not_Exist()
        {
            var userId = 1;

            _userRepositoryMock
                .Setup(x => x.GetByIdCheck(userId))
                .ReturnsAsync((UserEntity?)null);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.GetById(userId);

            Assert.False(result.Success);
            Assert.Equal($"Cannot retrieve user. User with id {userId} was not found.", result.Message);

            _userRepositoryMock.Verify(x => x.GetByIdCheck(userId), Times.Once);
            _repositoryUoWMock.Verify(x => x.Commit(), Times.Never);
            _transactionMock.Verify(x => x.Rollback(), Times.Once);
        }
    }
}