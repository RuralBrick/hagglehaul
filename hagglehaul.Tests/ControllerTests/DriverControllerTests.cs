using System.Security.Claims;
using System.Text;
using Azure.Core;
using hagglehaul.Server.Controllers;
using hagglehaul.Server.EmailViews;
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
    private Mock<IGeographicRouteService> _mockGeographicRouteService;
    private Mock<IEmailNotificationService> _mockEmailNotificationService;

    private DriverController _controller;
    
    [OneTimeSetUp]
    public void DriverControllerTestsSetup()
    {
        _mockDriverProfileService = new Mock<IDriverProfileService>();
        _mockRiderProfileService = new Mock<IRiderProfileService>();
        _mockUserCoreService = new Mock<IUserCoreService>();
        _mockTripService = new Mock<ITripService>();
        _mockBidService = new Mock<IBidService>();
        _mockGeographicRouteService = new Mock<IGeographicRouteService>();
        _mockEmailNotificationService = new Mock<IEmailNotificationService>();

        _controller = new DriverController(
            _mockDriverProfileService.Object,
            _mockRiderProfileService.Object,
            _mockUserCoreService.Object,
            _mockTripService.Object,
            _mockBidService.Object,
            _mockGeographicRouteService.Object,
            _mockEmailNotificationService.Object
        );
    }
    
    [SetUp]
    public void DriverControllerTestsSetupPerTest()
    {
        _mockDriverProfileService.Reset();
        _mockRiderProfileService.Reset();
        _mockUserCoreService.Reset();
        _mockTripService.Reset();
        _mockBidService.Reset();
        _mockGeographicRouteService.Reset();
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
    public async Task DriverFilterTripMarketByMaxEndToTargetDistance()
    {
        _mockTripService.Setup(
            x => x.GetAllTripsAsync()
        )!.ReturnsAsync(
            new List<Trip>
            {
                new Trip
                {
                    Id = "1",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 2,
                    DestinationLong = 0,
                },
                new Trip
                {
                    Id = "2",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 1,
                    DestinationLong = 0,
                },
                new Trip
                {
                    Id = "3",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 3,
                    DestinationLong = 0,
                },
            }
        );
        
        _mockBidService.Setup(
            x => x.GetTripBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>(0)
        );

        _mockGeographicRouteService.Setup(
            x => x.GetGeographicRoute(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())
        )!.ReturnsAsync(
            (double sLong, double sLat, double eLong, double eLat) => new GeographicRoute
            {
                Distance = Math.Abs(eLong - sLong) + Math.Abs(eLat - sLat),
            }
        );
        
        var tripsResult = await _controller.GetAllAvailableTrips(new TripMarketOptions
        {
            TargetLat = 0.0,
            TargetLong = 0.0,
            MaxEndToTargetDistance = 2.0
        });

        Assert.That(tripsResult, Is.TypeOf<OkObjectResult>());
        var trips = ((OkObjectResult) tripsResult).Value as List<SearchedTrip>;

        for (int i = 0; i < trips.Count; i++)
        {
            Assert.That(trips[i].TripId, Is.AnyOf(["1", "2"]));
        }
        
    }

    [Test]
    public async Task DriverFilterTripMarketByMaxEndToTargetDistanceAndMaxEuclideanDistance()
    {
        _mockTripService.Setup(
            x => x.GetAllTripsAsync()
        )!.ReturnsAsync(
            new List<Trip>
            {
                new Trip
                {
                    Id = "1",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 2,
                    DestinationLong = 0,
                    StartTime = DateTime.Now.AddHours(36),
                },
                new Trip
                {
                    Id = "2",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 1,
                    DestinationLong = 0,
                    StartTime = DateTime.Now.AddHours(36),
                },
                new Trip
                {
                    Id = "3",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 3,
                    DestinationLong = 0,
                    StartTime = DateTime.Now.AddHours(36),
                },
            }
        );

        _mockGeographicRouteService.Setup(
            x => x.GetGeographicRoute(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())
        )!.ReturnsAsync(
            (double sLong, double sLat, double eLong, double eLat) => new GeographicRoute
            {
                Distance = Math.Abs(eLong - sLong) + Math.Abs(eLat - sLat),
            }
        );
        
        _mockBidService.Setup(
            x => x.GetTripBidsAsync(It.IsAny<string>())
            )!.ReturnsAsync(
                (string s) => new List<Bid>(0)
            );

        var tripsResult = await _controller.GetAllAvailableTrips(new TripMarketOptions
        {
            TargetLat = 3.0,
            TargetLong = 0.0,
            MaxEndToTargetDistance = 1.0,
            MaxEuclideanDistance = 2.0
        });
        Assert.That(tripsResult, Is.TypeOf<OkObjectResult>());
        var trips = ((OkObjectResult) tripsResult).Value as List<SearchedTrip>;

        Assert.That(trips.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task DriverSortTripMarketByRouteDistance()
    {
        _mockTripService.Setup(
            x => x.GetAllTripsAsync()
        )!.ReturnsAsync(
            new List<Trip>
            {
                new Trip
                {
                    Id = "1",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 2,
                    DestinationLong = 2,
                    StartTime = DateTime.Now.AddHours(36),
                },
                new Trip
                {
                    Id = "2",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 1,
                    DestinationLong = 1,
                    StartTime = DateTime.Now.AddHours(36),
                },
                new Trip
                {
                    Id = "3",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 3,
                    DestinationLong = 3,
                    StartTime = DateTime.Now.AddHours(36),
                },
            }
        );

        _mockGeographicRouteService.Setup(
            x => x.GetGeographicRoute(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())
        )!.ReturnsAsync(
            (double sLong, double sLat, double eLong, double eLat) => new GeographicRoute
            {
                Distance = Math.Abs(eLong - sLong) + Math.Abs(eLat - sLat),
            }
        );
        
        _mockBidService.Setup(
            x => x.GetTripBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>(0)
        );
        
        var tripsResult = await _controller.GetAllAvailableTrips(new TripMarketOptions
        {
            SortMethods = [
                "routeDistance",
            ],
        });

        Assert.That(tripsResult, Is.TypeOf<OkObjectResult>());
        
        var trips = ((OkObjectResult) tripsResult).Value as List<SearchedTrip>;
        Assert.That(trips[0].TripId, Is.EqualTo("2"));
        Assert.That(trips[1].TripId, Is.EqualTo("1"));
        Assert.That(trips[2].TripId, Is.EqualTo("3"));
    }

    [Test]
    public async Task DriverSortTripMarketByRouteDistanceAndEndToTargetDistance()
    {
        _mockTripService.Setup(
            x => x.GetAllTripsAsync()
        )!.ReturnsAsync(
            new List<Trip>
            {
                new Trip
                {
                    Id = "2",
                    PickupLat = 1,
                    PickupLong = 0,
                    DestinationLat = 2,
                    DestinationLong = 0,
                },
                new Trip
                {
                    Id = "3",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 3,
                    DestinationLong = 0,
                },
                new Trip
                {
                    Id = "1",
                    PickupLat = 0,
                    PickupLong = 0,
                    DestinationLat = 1,
                    DestinationLong = 0,
                },
            }
        );

        _mockGeographicRouteService.Setup(
            x => x.GetGeographicRoute(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())
        )!.ReturnsAsync(
            (double sLong, double sLat, double eLong, double eLat) => new GeographicRoute
            {
                Distance = Math.Abs(eLong - sLong) + Math.Abs(eLat - sLat),
            }
        );

        _mockBidService.Setup(
            x => x.GetTripBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>(0)
        );
        
        var tripsResult = await _controller.GetAllAvailableTrips(new TripMarketOptions
        {
            TargetLat = 1,
            TargetLong = 0,
            SortMethods = [
                "routeDistance",
                "endToTargetDistance"
            ],
        });
        
        Assert.That(tripsResult, Is.TypeOf<OkObjectResult>());
        
        var trips = ((OkObjectResult) tripsResult).Value as List<SearchedTrip>;
        for (int i = 0; i < trips.Count; i++)
        {
            Assert.That(trips[i].TripId, Is.EqualTo((i + 1).ToString()));
        }
    }

    [Test]
    public async Task DriverFilterAndSortByCurrentMinBid()
    {
        _mockTripService.Setup(
            x => x.GetAllTripsAsync()
        )!.ReturnsAsync(
            new List<Trip>
            {
                new Trip
                {
                    Id = "2",
                    StartTime = DateTime.Now.AddHours(36),
                },
                new Trip
                {
                    Id = "3",
                    StartTime = DateTime.Now.AddHours(36),
                },
                new Trip
                {
                    Id = "1",
                    StartTime = DateTime.Now.AddHours(36),
                },
            }
        );

        _mockBidService.Setup(
            x => x.GetTripBidsAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new List<Bid>
            {
                new Bid
                {
                    Id = new StringBuilder().Insert(0, "1", 24).ToString(),
                    TripId = s,
                    CentsAmount = UInt32.Parse(s),
                    DriverEmail = "driver@example.com"
                }
            }
        );
        
        _mockDriverProfileService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new DriverProfile
            {
                Email = "driver@example.com",
            }
        );
        
        _mockGeographicRouteService.Setup(
            x => x.GetGeographicRoute(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())
        )!.ReturnsAsync(
            (double sLong, double sLat, double eLong, double eLat) => new GeographicRoute
            {
                Distance = Math.Abs(eLong - sLong) + Math.Abs(eLat - sLat),
            }
        );

        _mockUserCoreService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new UserCore
            {
                Email = "driver@example.com"
            }
        );
        
        var tripsResult = await _controller.GetAllAvailableTrips(new TripMarketOptions
        {
            MinCurrentMinBid = 2,
            SortMethods = [
                "currentMinBid"
            ],
        });
        Assert.That(tripsResult, Is.InstanceOf<OkObjectResult>());

        var trips = ((OkObjectResult) tripsResult).Value as List<SearchedTrip>;
        Assert.That(trips.Count, Is.EqualTo(2));

        for (int i = 0; i < trips.Count; i++)
        {
            Assert.That(trips[i].TripId, Is.EqualTo((i + 2).ToString()));
        }
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
        var trip = new Trip();
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) =>
            {
                trip = HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s);
                return trip;
            }
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

        _mockUserCoreService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => (
                s == "rider@example.com" ?
                new UserCore
                {
                    Email = "rider@example.com",
                    Phone = "1-800-RIDENOW",
                    Name = "Eebeedeebee",
                } :
                new UserCore
                {
                    Email = "driver@example.com",
                    Phone = "1-800-THISCAR",
                    Name = "Doobeedooba",
                }
            )
        );

        _mockDriverProfileService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string email) => new DriverProfile
            {
                Email = "driver@example.com",
                Rating = 4.20,
            }
        );

        string emailAddressed = String.Empty;
        NewBidEmail sentEmail = new NewBidEmail();
        _mockEmailNotificationService.Setup(
            x => x.SendEmailNotification(It.IsAny<EmailNotificationType>(), It.IsAny<string>(), It.IsAny<NewBidEmail>())
        )!.Callback(
            (EmailNotificationType _, string email, dynamic emailModel) =>
            {
                emailAddressed = email;
                sentEmail = emailModel as NewBidEmail;
            }
        );

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
        _mockUserCoreService.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Exactly(2));
        _mockDriverProfileService.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Exactly(1));
        _mockEmailNotificationService.Verify(x => x.SendEmailNotification(It.IsAny<EmailNotificationType>(), It.IsAny<string>(), It.IsAny<NewBidEmail>()), Times.Exactly(1));

        Assert.That(saveBid.TripId, Is.EqualTo(request.TripId));
        Assert.That(saveBid.CentsAmount, Is.EqualTo(request.CentsAmount));
        Assert.That(emailAddressed, Is.EqualTo("rider@example.com"));
        Assert.That(sentEmail.RiderName, Is.EqualTo("Eebeedeebee"));
        Assert.That(sentEmail.TripName, Is.EqualTo("MyTrip1"));
        Assert.That(sentEmail.DriverName, Is.EqualTo("Doobeedooba"));
        Assert.That(sentEmail.DriverRating, Is.EqualTo(4.20));
        Assert.That(sentEmail.Price, Is.EqualTo(1.00));
        Assert.That(sentEmail.StartTime, Is.EqualTo(trip.StartTime));
        Assert.That(sentEmail.PickupAddress, Is.EqualTo("123 Main St"));
        Assert.That(sentEmail.DestinationAddress, Is.EqualTo("456 Elm St"));
        Assert.That(sentEmail.RiderEmail, Is.EqualTo("rider@example.com"));
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
        var trip = new Trip();
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) =>
            {
                trip = HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s);
                return trip;
            }
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

        _mockUserCoreService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => (
                s == "rider@example.com" ?
                new UserCore
                {
                    Email = "rider@example.com",
                    Phone = "1-800-RIDENOW",
                    Name = "Eebeedeebee",
                } :
                new UserCore
                {
                    Email = "driver@example.com",
                    Phone = "1-800-THISCAR",
                    Name = "Doobeedooba",
                }
            )
        );

        _mockDriverProfileService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string email) => new DriverProfile
            {
                Email = "driver@example.com",
                Rating = 4.20,
            }
        );

        string emailAddressed = String.Empty;
        NewBidEmail sentEmail = new NewBidEmail();
        _mockEmailNotificationService.Setup(
            x => x.SendEmailNotification(It.IsAny<EmailNotificationType>(), It.IsAny<string>(), It.IsAny<NewBidEmail>())
        )!.Callback(
            (EmailNotificationType _, string email, dynamic emailModel) =>
            {
                emailAddressed = email;
                sentEmail = emailModel as NewBidEmail;
            }
        );

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
        Assert.That(emailAddressed, Is.EqualTo("rider@example.com"));
        Assert.That(sentEmail.RiderName, Is.EqualTo("Eebeedeebee"));
        Assert.That(sentEmail.TripName, Is.EqualTo("MyTrip1"));
        Assert.That(sentEmail.DriverName, Is.EqualTo("Doobeedooba"));
        Assert.That(sentEmail.DriverRating, Is.EqualTo(4.20));
        Assert.That(sentEmail.Price, Is.EqualTo(1.01));
        Assert.That(sentEmail.StartTime, Is.EqualTo(trip.StartTime));
        Assert.That(sentEmail.PickupAddress, Is.EqualTo("123 Main St"));
        Assert.That(sentEmail.DestinationAddress, Is.EqualTo("456 Elm St"));
        Assert.That(sentEmail.RiderEmail, Is.EqualTo("rider@example.com"));

        // Create a new bid
        request.TripId = new StringBuilder().Insert(0, "2", 24).ToString();
        Assert.That(await _controller.CreateOrUpdateBid(request), Is.InstanceOf<OkResult>());
        _mockBidService.Verify(x => x.CreateAsync(It.IsAny<Bid>()), Times.Once());
        // Update was not called any further
        Assert.That(emailAddressed, Is.EqualTo("rider@example.com"));
        Assert.That(sentEmail.RiderName, Is.EqualTo("Eebeedeebee"));
        Assert.That(sentEmail.TripName, Is.EqualTo("MyTrip2"));
        Assert.That(sentEmail.DriverName, Is.EqualTo("Doobeedooba"));
        Assert.That(sentEmail.DriverRating, Is.EqualTo(4.20));
        Assert.That(sentEmail.Price, Is.EqualTo(1.01));
        Assert.That(sentEmail.StartTime, Is.EqualTo(trip.StartTime));
        Assert.That(sentEmail.PickupAddress, Is.EqualTo("123 Main St"));
        Assert.That(sentEmail.DestinationAddress, Is.EqualTo("456 Elm St"));
        Assert.That(sentEmail.RiderEmail, Is.EqualTo("rider@example.com"));

        _mockUserCoreService.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Exactly(4));
        _mockDriverProfileService.Verify(x => x.GetAsync(It.IsAny<string>()), Times.Exactly(2));
        _mockEmailNotificationService.Verify(x => x.SendEmailNotification(It.IsAny<EmailNotificationType>(), It.IsAny<string>(), It.IsAny<NewBidEmail>()), Times.Exactly(2));
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

    [Test]
    public async Task DriverPostRating()
    {
        _mockTripService.Setup(
            x => x.GetTripByIdAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new Trip
            {
                Id = "testTrip",
                RiderEmail = "rider@example.com",
                DriverEmail = "driver@example.com",
                StartTime = DateTime.Now.AddHours(-12),
                RiderHasBeenRated = false,
                DriverHasBeenRated = false,
            }
        );

        Trip saveTrip = new Trip();
        _mockTripService.Setup(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Trip>())
        )!.Callback(
            (string s, Trip t) =>
            {
                saveTrip = t;
            }
        );

        _mockRiderProfileService.Setup(
            x => x.GetAsync(It.IsAny<string>())
        )!.ReturnsAsync(
            (string s) => new RiderProfile
            {
                Email = "rider@example.com",
                Rating = 4.0,
                NumRatings = 9,
            }
        );

        RiderProfile saveProfile = new RiderProfile();
        _mockRiderProfileService.Setup(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<RiderProfile>())
        )!.Callback(
            (string s, RiderProfile rp) =>
            {
                saveProfile = rp;
            }
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

        Assert.That(await _controller.RateRider(new GiveRating
        {
            TripId = "testTrip",
            RatingGiven = 5,
        }), Is.InstanceOf<OkResult>());
        _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<string>()), Times.Once());
        _mockRiderProfileService.Verify(x => x.GetAsync(It.IsAny<String>()), Times.Once());
        _mockRiderProfileService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<RiderProfile>()), Times.Once());
        _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Trip>()), Times.Once());

        Assert.That(saveProfile.Rating, Is.EqualTo(4.1));
        Assert.That(saveProfile.NumRatings, Is.EqualTo(10));

        Assert.That(saveTrip.RiderHasBeenRated, Is.EqualTo(true));
    }
}