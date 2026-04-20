using FCG.Platform.Application.Services;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace FCG.Platform.Test
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
                Name = "Pedro",
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
            _userRepositoryMock.Verify(x => x.Add(It.Is<UserEntity>(u =>
                u.Name == user.Name &&
                u.Email == user.Email &&
                u.Password == user.Password)), Times.Once);
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
                Name = "Pedro",
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
                Email = "old@email.com"
            };

            var updatedUser = new UserEntity
            {
                Id = 1,
                Name = "Novo Nome",
                Email = "novo@email.com"
            };

            _userRepositoryMock
                .Setup(x => x.GetById(user.Id))
                .ReturnsAsync(user);

            _repositoryUoWMock
                .Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _transactionMock
                .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(_repositoryUoWMock.Object);

            var result = await service.Update(updatedUser);

            Assert.True(result.Success);

            _userRepositoryMock.Verify(x => x.Update(It.Is<UserEntity>(u =>
                u.Name == updatedUser.Name &&
                u.Email == updatedUser.Email)), Times.Once);

            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Once);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Should_Throw_Exception_When_User_Not_Found()
        {
            var user = new UserEntity
            {
                Id = 1,
                Name = "Pedro",
                Email = "email@email.com"
            };

            _userRepositoryMock
                .Setup(x => x.GetById(user.Id))
                .ReturnsAsync((UserEntity?)null);

            var service = new UserService(_repositoryUoWMock.Object);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.Update(user));

            Assert.Contains("Failed to update user", exception.Message);

            _repositoryUoWMock.Verify(x => x.SaveAsync(), Times.Never);
            _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetExistingUserOrThrowAsync_Should_Return_User_When_User_Exists()
        {
            var userId = 1;

            var user = new UserEntity
            {
                Id = userId,
                Name = "Pedro",
                Email = "pedro@email.com"
            };

            _userRepositoryMock
                .Setup(x => x.GetById(userId))
                .ReturnsAsync(user);

            var service = new UserService(_repositoryUoWMock.Object);
            var result = await service.Update(user);

            Assert.True(result.Success);
        }
    }
}