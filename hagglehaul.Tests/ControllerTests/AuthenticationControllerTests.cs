using hagglehaul.Server.Controllers;
using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hagglehaul.Tests.ControllerTests
{
    internal class AuthenticationControllerTests
    {
        private Mock<IUserCoreService> _mockUserCoreService;
        private Mock<IRiderProfileService> _mockRiderProfileService;
        private Mock<IDriverProfileService> _mockDriverProfileService;

        private AuthenticationController _controller;

        [OneTimeSetUp]
        public void AuthenticationControllerTestsSetup()
        {
            _mockUserCoreService = new Mock<IUserCoreService>();
            _mockRiderProfileService = new Mock<IRiderProfileService>();
            _mockDriverProfileService = new Mock<IDriverProfileService>();
            var options = Options.Create(new JwtSettings
            {
                ValidIssuer = "issuer",
                ValidAudience = "audience",
                Secret = "secretsecretsecretsecretsecretsecret",
            });

            _controller = new AuthenticationController(
                               options,
                                              _mockUserCoreService.Object,
                                                             _mockRiderProfileService.Object,
                                                                            _mockDriverProfileService.Object
                                                                                       );
        }

        [SetUp]
        public void AuthenticationControllerTestsSetupPerTest()
        {
            _mockUserCoreService.Reset();
            _mockRiderProfileService.Reset();
            _mockDriverProfileService.Reset();
        }

        [Test]
        public async Task TestSuccessfulLogin()
        {
            _mockUserCoreService.Setup(x => x.GetAsync(It.IsAny<string>()))!.ReturnsAsync(new UserCore
            {
                Email = "rider@example.com",
                PasswordHash = "passwordsalt",
                Salt = "salt",
                Role = "rider",
            });

            _mockUserCoreService.Setup(x => x.ComparePasswordToHash(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))!.Returns((string password, string hash, string salt) => hash == password + salt);

            var response = await _controller.Login(new Login
            {
                Email = "rider@example.com",
                Password = "password",
            }) as OkObjectResult;
            _mockUserCoreService.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Once);
            _mockUserCoreService.Verify(x => x.ComparePasswordToHash(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            Assert.That(response, Is.TypeOf<OkObjectResult>());

            var token = response.Value?.GetType()?.GetProperty("token")?.GetValue(response.Value);
            var expiration = response.Value?.GetType()?.GetProperty("expiration")?.GetValue(response.Value);

            Assert.That(token, Is.Not.Null);
            Assert.That(expiration, Is.Not.Null);
        }

        [Test]
        public async Task TestSuccessfulRegister()
        {
            _mockUserCoreService.SetupSequence(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync((UserCore)null!)
                .ReturnsAsync(new UserCore
                {
                    Email = "rider@example.com",
                    PasswordHash = "passwordsalt",
                    Salt = "salt",
                    Role = "rider",
                });

            _mockUserCoreService.Setup(x => x.CreatePasswordHash(It.IsAny<string>(), out It.Ref<string>.IsAny, out It.Ref<string>.IsAny))!.Callback((string password, out string hash, out string salt) =>
            {
                salt = "salt";
                hash = password + salt;
            });

            _mockUserCoreService.Setup(x => x.CreateAsync(It.IsAny<UserCore>()))!.Callback((UserCore savedUser) =>
            {
                Assert.That(savedUser.Email, Is.EqualTo("rider@example.com"));
                Assert.That(savedUser.PasswordHash, Is.EqualTo("passwordsalt"));
                Assert.That(savedUser.Salt, Is.EqualTo("salt"));
                Assert.That(savedUser.Role, Is.EqualTo("rider"));
            });

            _mockUserCoreService.Setup(x => x.ComparePasswordToHash(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))!.Returns((string password, string hash, string salt) => hash == password + salt);

            _mockRiderProfileService.Setup(x => x.CreateAsync(It.IsAny<RiderProfile>()))!.ReturnsAsync((RiderProfile)null!);

            _mockDriverProfileService.Setup(x => x.CreateAsync(It.IsAny<DriverProfile>()))!.ReturnsAsync((DriverProfile)null!);

            var actualResponse = await _controller.Register(new Register
            {
                Email = "rider@example.com",
                Password = "password",
                Role = "rider",
            });

            Assert.That(actualResponse, Is.TypeOf<OkObjectResult>());

            var response = actualResponse as OkObjectResult;

            _mockUserCoreService.Verify(x => x.GetAsync(It.IsAny<string>()), Times.AtLeastOnce);
            _mockUserCoreService.Verify(x => x.CreatePasswordHash(It.IsAny<string>(), out It.Ref<string>.IsAny, out It.Ref<string>.IsAny), Times.Once);
            _mockUserCoreService.Verify(x => x.CreateAsync(It.IsAny<UserCore>()), Times.Once);
            _mockRiderProfileService.Verify(x => x.CreateAsync(It.IsAny<RiderProfile>()), Times.Once);
            _mockDriverProfileService.Verify(x => x.CreateAsync(It.IsAny<DriverProfile>()), Times.Never);
            _mockUserCoreService.Verify(x => x.ComparePasswordToHash(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            Assert.That(response, Is.TypeOf<OkObjectResult>());

            var token = response.Value?.GetType()?.GetProperty("token")?.GetValue(response.Value);
            var expiration = response.Value?.GetType()?.GetProperty("expiration")?.GetValue(response.Value);

            Assert.That(token, Is.Not.Null);
            Assert.That(expiration, Is.Not.Null);
        }
    }
}
