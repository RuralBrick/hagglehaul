using System.Security.Claims;
using Moq;
using hagglehaul.Server.Services;
using hagglehaul.Server.Controllers;
using hagglehaul.Tests.SharedHelpers;
using hagglehaul.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hagglehaul.Tests.ControllerTests;

public class MongoTestControllerTests
{
    private Mock<IMongoTestService> _mockMongoTestService;
    private MongoTestController _controller;

    [OneTimeSetUp]
    public void MongoTestControllerTestsSetup()
    {
        _mockMongoTestService = new Mock<IMongoTestService>();
        _controller = new MongoTestController(_mockMongoTestService.Object);
    }
    
    [Test]
    public async Task GetTest()
    {
        _mockMongoTestService.Setup(x => x.GetAsync()).ReturnsAsync(HhTestUtilities.GetMongoTestData());
        
        var actual = await _controller.Get();
        Assert.IsTrue(HhTestUtilities.CompareJson(actual, HhTestUtilities.GetMongoTestData()));
    }
    
    [Test]
    public async Task GetSecureTest()
    {
        _mockMongoTestService.Setup(x => x.GetAsync()).ReturnsAsync(HhTestUtilities.GetMongoTestData());
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "mock@example.com"),
            new Claim(ClaimTypes.Role, "rider")
        }, "mock"));

        string userAddenum = " Username: mock@example.com Role: rider";
        var expected = HhTestUtilities.GetMongoTestData();
        expected.ForEach(x => x.Test += userAddenum);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        var actual = await _controller.GetSecure();
        Assert.IsTrue(HhTestUtilities.CompareJson(actual, expected));
    }
}
