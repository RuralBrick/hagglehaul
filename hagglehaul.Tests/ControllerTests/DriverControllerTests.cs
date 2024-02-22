using System.Security.Claims;
using System.Text;
using Azure.Core;
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
    private Mock<IUserCoreService> _mockUserCoreService;
    private Mock<ITripService> _mockTripService;
    private Mock<IBidService> _mockBidService;
    
    private DriverController _controller;
    
    [OneTimeSetUp]
    public void DriverControllerTestsSetup()
    {
        _mockDriverProfileService = new Mock<IDriverProfileService>();
        _mockRiderProfileService = new Mock<IRiderProfileService>();
        _mockUserCoreService = new Mock<IUserCoreService>();
        _mockTripService = new Mock<ITripService>();
        _mockBidService = new Mock<IBidService>();
        
        _controller = new DriverController(_mockDriverProfileService.Object, _mockRiderProfileService.Object, _mockUserCoreService.Object, _mockTripService.Object, _mockBidService.Object);
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
    public async Task DriverUpdateTest()
    {
        _mockUserCoreService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new UserCore
            {
                Email = "goofywoofy@gmail.com",
                PasswordHash = "123456",
            }
        );

        _mockUserCoreService.Setup(
            x => x.ComparePasswordToHash(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())
        )!.Returns(true);

        _mockUserCoreService.Setup(
            x => x.CreatePasswordHash(It.IsAny<string>(), out It.Ref<string>.IsAny, out It.Ref<string>.IsAny)
        )!.Callback(
            (string password, out string hash, out string salt) =>
            {
                hash = password;
                salt = "nosalt";
            }
        );

        UserCore savedUserCore = null!;
        _mockUserCoreService.Setup(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<UserCore>())
        )!.Callback(
            (string s, UserCore uc) => savedUserCore = uc
        );

        _mockDriverProfileService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new DriverProfile
            {
                Email = "goofywoofy@gmail.com",
            }
        );

        DriverProfile savedDriverProfile = null!;
        _mockDriverProfileService.Setup(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<DriverProfile>())
        )!.Callback(
            (string s, DriverProfile dp) => savedDriverProfile = dp
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "goofywoofy@gmail.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        Assert.That(await _controller.ModifyAccountDetails(new DriverUpdate
        {
            Name = "",
            Phone = "",
            CarDescription = "",
            CurrentPassword = "123456",
            NewPassword = "1234567",
        }), Is.TypeOf<OkResult>());

        Assert.That(savedUserCore.PasswordHash, Is.EqualTo("1234567"));
    }
  
    [Test]
    public async Task DriverGetOwnBidsTest()
    {
        var driverBidData = HhTestUtilities.GetBidData()
                                           .Where(bid => bid.DriverEmail == "driver@example.com")
                                           .ToList();
        _mockBidService.Setup(
            x => x.GetDriverBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => driverBidData
        );

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await _controller.GetDriverBids() as OkObjectResult;

        Assert.That(result, Is.TypeOf<OkObjectResult>());

        var driverBids = result.Value as List<Bid>;

        Assert.That(driverBids, Is.TypeOf<List<Bid>>());

        for (int i = 0; i < driverBids.Count; i++)
        {
            Assert.That(driverBids[i].DriverEmail, Is.EqualTo("driver@example.com"));
            Assert.That(driverBids[i].TripId, Is.EqualTo(driverBidData[i].TripId));
            Assert.That(driverBids[i].CentsAmount, Is.EqualTo(driverBidData[i].CentsAmount));
        }
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

        var request = new CreateOrUpdateBid
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
        
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<OkResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Once());
        
        Assert.That(saveBid.TripId, Is.EqualTo(request.TripId));
        Assert.That(saveBid.CentsAmount, Is.EqualTo(request.CentsAmount));
    }
    
    [Test]
    public async Task DriverControllerCannotCreateBidPastOrConfirmedTripTest()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData(hasDriver: true).FirstOrDefault(trip => trip.Id == s)
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

        var request = new CreateOrUpdateBid
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
        
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
        
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData(inPast: true).FirstOrDefault(trip => trip.Id == s)
        );
        
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
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

        var request = new CreateOrUpdateBid
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
        
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<UnauthorizedResult>());
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

        var request = new CreateOrUpdateBid
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
        
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<BadRequestObjectResult>());
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

        var request = new CreateOrUpdateBid
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
        
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
    }
    
    [Test]
    public async Task DriverControllerCanUpdateBidTest()
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

        string saveId = String.Empty;
        Bid saveBid = new Bid();
        _mockBidService.Setup(
            x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Bid>())
        )!.Callback<String, Bid>((String id, Bid b) =>
        {
            saveId = id;
            saveBid = b;
        });

        var request = new CreateOrUpdateBid
        {
            TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
            CentsAmount = 101
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
        
        // Modify a bid
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<OkResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Never());
        _mockBidService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Bid>()), Times.Once());
        Assert.That(saveId, Is.EqualTo(request.TripId));
        Assert.That(saveBid.TripId, Is.EqualTo(request.TripId));
        Assert.That(saveBid.CentsAmount, Is.EqualTo(request.CentsAmount));
        
        // Create a new bid
        request.TripId = new StringBuilder().Insert(0, "2", 24).ToString();
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<OkResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Once());
        // Update was not called any further
        _mockBidService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Bid>()), Times.Once());
    }

    [Test]
    public async Task DriverControllerDeleteBidTest()
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

        string saveId = String.Empty;
        _mockBidService.Setup(
            x => x.DeleteAsync(It.IsAny<String>())
        )!.Callback<String>((String id) => saveId = id);

        string requestId = new StringBuilder().Insert(0, "1", 24).ToString();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        Assert.That(await _controller.DeleteBid(requestId), Is.InstanceOf<OkResult>());
        _mockBidService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Once());
        Assert.That(saveId, Is.EqualTo(requestId));
    }

    [Test]
    public async Task DriverControllerCannotDeleteNonexistentBidTest()
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

        string saveId = String.Empty;
        _mockBidService.Setup(
            x => x.DeleteAsync(It.IsAny<String>())
        )!.Callback<String>((String id) => saveId = id);

        string requestId = new StringBuilder().Insert(0, "1", 24).ToString();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        Assert.That(await _controller.DeleteBid(requestId), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Never());
    }

    [Test]
    public async Task DriverControllerCannotDeletePastOrConfirmedTripBidTest()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData(hasDriver: true).FirstOrDefault(trip => trip.Id == s)
        );

        _mockBidService.Setup(
            x => x.GetDriverBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetBidData(1)
        );

        string saveId = String.Empty;
        _mockBidService.Setup(
            x => x.DeleteAsync(It.IsAny<String>())
        )!.Callback<String>((String id) => saveId = id);

        string requestId = new StringBuilder().Insert(0, "1", 24).ToString();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "driver@example.com"),
            new Claim(ClaimTypes.Role, "driver")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        Assert.That(await _controller.DeleteBid(requestId), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Never());

        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => HhTestUtilities.GetTripData(inPast: true).FirstOrDefault(trip => trip.Id == s)
        );

        Assert.That(await _controller.DeleteBid(requestId), Is.InstanceOf<BadRequestObjectResult>());
        _mockBidService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Never());
    }
}