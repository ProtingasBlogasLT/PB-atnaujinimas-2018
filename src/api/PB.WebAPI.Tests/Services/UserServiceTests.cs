using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using PB.WebAPI.Models;
using PB.WebAPI.Repositories;
using PB.WebAPI.Services;

namespace PB.WebAPI.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        [Test]
        public void CreateUserAsync_CreatesUser()
        {
            const string hash = "ABC123";
            var user = new User();
            var passwordMock = new Mock<IPasswordService>();
            passwordMock.Setup(srv => srv.HashPassword(It.IsAny<string>())).Returns(hash);
            var repoMock = new Mock<IUsersRepo>();

            string receivedHash = null;
            void Callback(User u)
            {
                receivedHash = u.HashedPassword;
            }
            repoMock.Setup(rep => rep.CreateAsync(It.IsAny<User>())).Callback((Action<User>) Callback).Returns(Task.CompletedTask);
            var service = new UserService(null, repoMock.Object, passwordMock.Object);

            service.CreateUserAsync(user).Wait();

            repoMock.Verify(srv => srv.CreateAsync(It.Is<User>(u => u == user)), Times.Once);
            Assert.That(receivedHash == hash);
            repoMock.VerifyAll();
        }

        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void AuthenticateAsync_ValidatesPassword(bool userExists, bool authenticate)
        {
            var userHashedPassword = "userHashedPassword";
            var username = "username";
            var password = "password";
            var userId = 123;
            var token = "token";

            var user = userExists ? new User {Id = userId, HashedPassword = userHashedPassword} : null;

            var tokenMock = new Mock<ITokenService>();
            var settings = new AppSettings();
            settings.TokenSecret = "my very big secret";
            tokenMock.Setup(t => t.GenerateToken(userId)).Returns(token);
            string receivedHash = null;
            void Callback(string u, string h)
            {
                receivedHash = h;
            }
            var passwordMock = new Mock<IPasswordService>();
            passwordMock.Setup(srv => srv.ValidatePassword(password, userHashedPassword)).Callback((Action<string, string>)Callback).Returns(authenticate);
            var repoMock = new Mock<IUsersRepo>();
            repoMock.Setup(rep => rep.ReadAsync(username)).Returns(Task.FromResult(user));
            var service = new UserService(tokenMock.Object, repoMock.Object, passwordMock.Object);

            var returnedUser = service.AuthenticateAsync(username, password).Result;

            repoMock.Verify(rep => rep.ReadAsync(username), Times.Once);

            if (!userExists)
            {
                passwordMock.Verify(srv => srv.ValidatePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
                Assert.That(returnedUser == null);
                return;
            }
            passwordMock.Verify(srv => srv.ValidatePassword(password, It.IsAny<string>()), Times.Once);
            if (!authenticate)
            {
                Assert.That(returnedUser == null);
                return;
            }
            tokenMock.Verify(t => t.GenerateToken(userId), Times.Once);
            Assert.That(receivedHash == userHashedPassword);
            Assert.That(user == returnedUser);
            Assert.That(user.Token == token);
        }
    }
}
