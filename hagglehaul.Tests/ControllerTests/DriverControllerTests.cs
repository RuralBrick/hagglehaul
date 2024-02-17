using System.Security.Claims;
using System.Text;
using hagglehaul.Server.Controllers;
using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace hagglehaul.Tests.ControllerTests;

public class DriverControllerTests
{
    private Mock<IDriverProfileService> _mockDriverProfileService;
    private Mock<IRiderProfileService> _mockRiderProfileService;
    private Mock<ITripService> _mockTripService;
    private Mock<IBidService> _mockBidService;
    
    private DriverController _controller;
    
    [OneTimeSetUp]
    public void DriverControllerTestsSetup()
    {
        _mockDriverProfileService = new Mock<IDriverProfileService>();
        _mockRiderProfileService = new Mock<IRiderProfileService>();
        _mockTripService = new Mock<ITripService>();
        _mockBidService = new Mock<IBidService>();
        
        _controller = new DriverController(_mockDriverProfileService.Object, _mockRiderProfileService.Object, _mockTripService.Object, _mockBidService.Object);
    }
    
    [SetUp]
    public void DriverControllerTestsSetupPerTest()
    {
        _mockDriverProfileService.Reset();
        _mockRiderProfileService.Reset();
        _mockTripService.Reset();
        _mockBidService.Reset();
    }

    [Test]
    public async Task DriverControllerCreateBidTest()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s)
        );
        
        _mockBidService.Setup(
            x => x.GetDriverBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>(0)
        );
        
        Bid saveBid = new Bid();
        _mockBidService.Setup(
            x => x.CreateAsync(It.IsAny<Bid>())
        )!.Callback<Bid>((Bid b) => saveBid = b);

        var request = new CreateBid
        {
            TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
            CentsAmount = 100
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        Assert.That(await _controller.CreateBid(request), Is.InstanceOf<OkResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Once());
        
        Assert.That(saveBid.TripId, Is.EqualTo(request.TripId));
        Assert.That(saveBid.CentsAmount, Is.EqualTo(request.CentsAmount));
    }
    
    [Test]
    public async Task DriverControllerCannotCreateBidAsRiderTest()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s)
        );
        
        _mockBidService.Setup(
            x => x.GetDriverBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>(0)
        );

        var request = new CreateBid
        {
            TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
            CentsAmount = 100
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "rider@example.com"),
            new Claim(ClaimTypes.Role, "rider")
        }, "mock"));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        Assert.That(await _controller.CreateBid(request), Is.InstanceOf<UnauthorizedResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
    }
    
    [Test]
    public async Task DriverControllerCannotCreateBidForBadTripTest()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s)
        );
        
        _mockBidService.Setup(
            x => x.GetDriverBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>(0)
        );

        var request = new CreateBid
        {
            TripId = new StringBuilder().Insert(0, "9", 24).ToString(), // nonexistent trip
            CentsAmount = 100
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        Assert.That(await _controller.CreateBid(request), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
    }
    
    [Test]
    public async Task DriverControllerCannotCreateBidWithBadAmountTest()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s)
        );
        
        _mockBidService.Setup(
            x => x.GetDriverBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>(0)
        );

        var request = new CreateBid
        {
            TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
            CentsAmount = 0
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        Assert.That(await _controller.CreateBid(request), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
    }
    
    [Test]
    public async Task DriverControllerCannotCreateTwoBidsOnTripTest()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s)
        );
        
        _mockBidService.Setup(
            x => x.GetDriverBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetBidData(1)
        );
        
        Bid saveBid = new Bid();
        _mockBidService.Setup(
            x => x.CreateAsync(It.IsAny<Bid>())
        )!.Callback<Bid>((Bid b) => saveBid = b);

        var request = new CreateBid
        {
            TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
            CentsAmount = 100
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        
        Assert.That(await _controller.CreateBid(request), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
        
        request.CentsAmount = 101;
        Assert.That(await _controller.CreateBid(request), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never);
        
        request.TripId = new StringBuilder().Insert(0, "2", 24).ToString();
        Assert.That(await _controller.CreateBid(request), Is.InstanceOf<OkResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Once());
        Assert.That(saveBid.TripId, Is.EqualTo(request.TripId));
    }
}
