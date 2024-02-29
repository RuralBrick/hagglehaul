using Azure.Core;
using hagglehaul.Server.Controllers;
using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace hagglehaul.Tests.ControllerTests
{
    internal class RiderControllerTests
    {
        private Mock<IRiderProfileService> _mockRiderProfileService;
        private Mock<IDriverProfileService> _mockDriverProfileService;
        private Mock<IUserCoreService> _mockUserCoreService;
        private Mock<ITripService> _mockTripService;
        private Mock<IBidService> _mockBidService;
        private Mock<IGeographicRouteService> _mockGeographicRouteService;

        private RiderController _controller;

        [OneTimeSetUp]
        public void DriverControllerTestsSetup()
        {
            _mockRiderProfileService = new Mock<IRiderProfileService>();
            _mockDriverProfileService = new Mock<IDriverProfileService>();
            _mockUserCoreService = new Mock<IUserCoreService>();
            _mockTripService = new Mock<ITripService>();
            _mockBidService = new Mock<IBidService>();
            _mockGeographicRouteService = new Mock<IGeographicRouteService>();

            _controller = new RiderController(
                _mockRiderProfileService.Object,
                _mockDriverProfileService.Object,
                _mockUserCoreService.Object,
                _mockTripService.Object,
                _mockBidService.Object,
                _mockGeographicRouteService.Object
            );
        }

        [SetUp]
        public void DriverControllerTestsSetupPerTest()
        {
            _mockRiderProfileService.Reset();
            _mockDriverProfileService.Reset();
            _mockTripService.Reset();
            _mockBidService.Reset();
        }

        [Test]
        public async Task RiderPostTripTest()
        {
            Trip saveTrip = new Trip();
            _mockTripService.Setup(
                x => x.CreateAsync(It.IsAny<Trip>())
            )!.Callback<Trip>((Trip t) => saveTrip = t);

            var request = new CreateTrip
            {
                Name = "Road Trip",
                StartTime = DateTime.Now.AddDays(1),
                PickupLat = 34.050,
                PickupLong = -118.250,
                DestinationLat = 40.731,
                DestinationLong = -73.935,
                PickupAddress = "123 Main St",
                DestinationAddress = "456 Elm St",
                PartySize = 3
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

            Assert.That(await _controller.PostRiderTrip(request), Is.InstanceOf<OkResult>());
            _mockTripService.Verify(x => x.CreateAsync(It.IsAny<Trip>()), Times.Once());

            Assert.That(saveTrip.RiderEmail, Is.EqualTo("rider@example.com"));
            Assert.That(saveTrip.Name, Is.EqualTo(request.Name));
            Assert.That(saveTrip.StartTime, Is.EqualTo(request.StartTime));
            Assert.That(saveTrip.PickupLat, Is.EqualTo(request.PickupLat));
            Assert.That(saveTrip.PickupLong, Is.EqualTo(request.PickupLong));
            Assert.That(saveTrip.DestinationLat, Is.EqualTo(request.DestinationLat));
            Assert.That(saveTrip.DestinationLong, Is.EqualTo(request.DestinationLong));
            Assert.That(saveTrip.PickupAddress, Is.EqualTo(request.PickupAddress));
            Assert.That(saveTrip.DestinationAddress, Is.EqualTo(request.DestinationAddress));
            Assert.That(saveTrip.PartySize, Is.EqualTo(request.PartySize));
            Assert.False(saveTrip.RiderHasBeenRated);
            Assert.False(saveTrip.DriverHasBeenRated);
        }
        
         [Test]
        public async Task RiderCannotPostInvalidTripTest()
        {
            Trip saveTrip = new Trip();
            _mockTripService.Setup(
                x => x.CreateAsync(It.IsAny<Trip>())
            )!.Callback<Trip>((Trip t) => saveTrip = t);

            var request = new CreateTrip
            {
                Name = "Road Trip",
                StartTime = DateTime.Now.AddDays(-1),
                PickupLat = 34.050,
                PickupLong = -118.250,
                DestinationLat = 40.731,
                DestinationLong = -73.935,
                PickupAddress = "123 Main St",
                DestinationAddress = "456 Elm St",
                PartySize = 3
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

            Assert.That(await _controller.PostRiderTrip(request), Is.InstanceOf<BadRequestObjectResult>());
            _mockTripService.Verify(x => x.CreateAsync(It.IsAny<Trip>()), Times.Never);
            
            
            request.StartTime = DateTime.Now.AddDays(1);
            request.PartySize = 0;
            Assert.That(await _controller.PostRiderTrip(request), Is.InstanceOf<BadRequestObjectResult>());
            _mockTripService.Verify(x => x.CreateAsync(It.IsAny<Trip>()), Times.Never);
        }

        [Test]
        public async Task RiderDeleteTripTest()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) => 
                HhTestUtilities.GetTripData(1, false, false).FirstOrDefault());
            
            _mockTripService.Setup(
                x => x.DeleteAsync(It.IsAny<String>())
            )!.Callback((string s) =>
            {
                Assert.That(s, Is.EqualTo(new StringBuilder().Insert(0, "1", 24).ToString()));
            });
            
            _mockBidService.Setup(
                x => x.DeleteByTripIdAsync(It.IsAny<String>())
            )!.Callback((string s) =>
            {
                Assert.That(s, Is.EqualTo(new StringBuilder().Insert(0, "1", 24).ToString()));
            });
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            
            Assert.That(await _controller.DeleteRiderTrip(new StringBuilder().Insert(0, "1", 24).ToString()), Is.InstanceOf<OkResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Once());
            _mockBidService.Verify(x => x.DeleteByTripIdAsync(It.IsAny<String>()), Times.Once());
        }

        [Test]
        public async Task DeleteTripWrongUserTest()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetTripData(1, false, false).FirstOrDefault());

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "imposter@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.DeleteRiderTrip(new StringBuilder().Insert(0, "1", 24).ToString()),
                Is.InstanceOf<UnauthorizedResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Never());
            _mockBidService.Verify(x => x.DeleteByTripIdAsync(It.IsAny<String>()), Times.Never());
        }

        [Test]
        public async Task DeleteTripUnauthorizedTest()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "driver@example.com"),
                new Claim(ClaimTypes.Role, "driver")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            
            Assert.That(await _controller.DeleteRiderTrip(new StringBuilder().Insert(0, "1", 24).ToString()),
                Is.InstanceOf<UnauthorizedResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Never());
            _mockTripService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Never());
            _mockBidService.Verify(x => x.DeleteByTripIdAsync(It.IsAny<String>()), Times.Never());
        }

        [Test]
        public async Task CannotDeletePastOrConfirmedTripTest()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) => 
                HhTestUtilities.GetTripData(1, true, false).FirstOrDefault());
            
            _mockTripService.Setup(
                x => x.DeleteAsync(It.IsAny<String>())
            )!.Callback((string s) =>
            {
                Assert.That(s, Is.EqualTo(new StringBuilder().Insert(0, "1", 24).ToString()));
            });
            
            _mockBidService.Setup(
                x => x.DeleteByTripIdAsync(It.IsAny<String>())
            )!.Callback((string s) =>
            {
                Assert.That(s, Is.EqualTo(new StringBuilder().Insert(0, "1", 24).ToString()));
            });
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            
            Assert.That(await _controller.DeleteRiderTrip(new StringBuilder().Insert(0, "1", 24).ToString()), Is.InstanceOf<BadRequestObjectResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Never());
            _mockBidService.Verify(x => x.DeleteByTripIdAsync(It.IsAny<String>()), Times.Never());
            
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) => 
                HhTestUtilities.GetTripData(1, false, true).FirstOrDefault());
            
            Assert.That(await _controller.DeleteRiderTrip(new StringBuilder().Insert(0, "1", 24).ToString()), Is.InstanceOf<BadRequestObjectResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Exactly(2));
            _mockTripService.Verify(x => x.DeleteAsync(It.IsAny<String>()), Times.Never());
            _mockBidService.Verify(x => x.DeleteByTripIdAsync(It.IsAny<String>()), Times.Never());
        }

        [Test]
        public async Task RiderConfirmDriverTest()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetTripData(1, false, false).FirstOrDefault());

            _mockTripService.Setup(
                x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>())
            )!.Callback((string s, Trip t) =>
            {
                Assert.That(s, Is.EqualTo(new StringBuilder().Insert(0, "1", 24).ToString()));
                Assert.That(t.DriverEmail, Is.EqualTo("driver@example.com"));
            });

            _mockBidService.Setup(
                x => x.GetTripBidsAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetBidData(2, false).ToList()
            );
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.ConfirmDriver(new AddTripDriver
            {
                TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
                BidId = new StringBuilder().Insert(0, "2", 24).ToString()
            }), Is.InstanceOf<OkResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>()), Times.Once());
            _mockBidService.Verify(x => x.GetTripBidsAsync(It.IsAny<String>()), Times.Once());
        }
        
        [Test]
        public async Task RiderWrongUserConfirmDriverTest()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetTripData(1, false, false).FirstOrDefault());
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "imposter@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.ConfirmDriver(new AddTripDriver
            {
                TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
                BidId = new StringBuilder().Insert(0, "2", 24).ToString()
            }), Is.InstanceOf<UnauthorizedResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>()), Times.Never());
            _mockBidService.Verify(x => x.GetTripBidsAsync(It.IsAny<String>()), Times.Never());
        }
        
        [Test]
        public async Task RiderUnauthorizedConfirmDriverTest()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "driver@example.com"),
                new Claim(ClaimTypes.Role, "driver")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.ConfirmDriver(new AddTripDriver
            {
                TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
                BidId = new StringBuilder().Insert(0, "2", 24).ToString()
            }), Is.InstanceOf<UnauthorizedResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Never());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>()), Times.Never());
            _mockBidService.Verify(x => x.GetTripBidsAsync(It.IsAny<String>()), Times.Never());
        }
        
        [Test]
        public async Task RiderConfirmDriverTestNonExistentTrip()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                null);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.ConfirmDriver(new AddTripDriver
            {
                TripId = new StringBuilder().Insert(0, "3", 24).ToString(),
                BidId = new StringBuilder().Insert(0, "2", 24).ToString()
            }), Is.InstanceOf<BadRequestObjectResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>()), Times.Never());
            _mockBidService.Verify(x => x.GetTripBidsAsync(It.IsAny<String>()), Times.Never());
        }
        
        [Test]
        public async Task RiderConfirmDriverTestNonExistentBid()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetTripData(1, false, false).FirstOrDefault());

            _mockTripService.Setup(
                x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>())
            )!.Callback((string s, Trip t) =>
            {
                Assert.That(s, Is.EqualTo(new StringBuilder().Insert(0, "1", 24).ToString()));
                Assert.That(t.DriverEmail, Is.EqualTo("driver@example.com"));
            });

            _mockBidService.Setup(
                x => x.GetTripBidsAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetBidData(2, false).ToList()
            );
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.ConfirmDriver(new AddTripDriver
            {
                TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
                BidId = new StringBuilder().Insert(0, "3", 24).ToString()
            }), Is.InstanceOf<BadRequestObjectResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>()), Times.Never());
            _mockBidService.Verify(x => x.GetTripBidsAsync(It.IsAny<String>()), Times.Once());
        }
        
        [Test]
        public async Task RiderConfirmDriverPastOrConfirmedTripTest()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetTripData(1, true, false).FirstOrDefault());

            _mockTripService.Setup(
                x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>())
            )!.Callback((string s, Trip t) =>
            {
                Assert.That(s, Is.EqualTo(new StringBuilder().Insert(0, "1", 24).ToString()));
                Assert.That(t.DriverEmail, Is.EqualTo("driver@example.com"));
            });

            _mockBidService.Setup(
                x => x.GetTripBidsAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetBidData(2, false).ToList()
            );
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.ConfirmDriver(new AddTripDriver
            {
                TripId = new StringBuilder().Insert(0, "1", 24).ToString(),
                BidId = new StringBuilder().Insert(0, "2", 24).ToString()
            }), Is.InstanceOf<BadRequestObjectResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>()), Times.Never());
            
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<String>())
            )!.ReturnsAsync((string s) =>
                HhTestUtilities.GetTripData(1, false, true).FirstOrDefault());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<String>()), Times.Once());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<Trip>()), Times.Never());
        }

        [Test]
        public async Task RiderPostRating()
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

            _mockDriverProfileService.Setup(
                x => x.GetAsync(It.IsAny<string>())
            )!.ReturnsAsync(
                (string s) => new DriverProfile
                {
                    Email = "driver@example.com",
                    Rating = 4.0,
                    NumRatings = 9,
                }
            );

            DriverProfile saveProfile = new DriverProfile();
            _mockDriverProfileService.Setup(
                x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<DriverProfile>())
            )!.Callback(
                (string s, DriverProfile dp) =>
                {
                    saveProfile = dp;
                }
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.RateDriver(new GiveRating
            {
                TripId = "testTrip",
                RatingGiven = 5,
            }), Is.InstanceOf<OkResult>());
            _mockTripService.Verify(x => x.GetTripByIdAsync(It.IsAny<string>()), Times.Once());
            _mockDriverProfileService.Verify(x => x.GetAsync(It.IsAny<String>()), Times.Once());
            _mockDriverProfileService.Verify(x => x.UpdateAsync(It.IsAny<String>(), It.IsAny<DriverProfile>()), Times.Once());
            _mockTripService.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Trip>()), Times.Once());

            Assert.That(saveProfile.Rating, Is.EqualTo(4.1));
            Assert.That(saveProfile.NumRatings, Is.EqualTo(10));

            Assert.That(saveTrip.DriverHasBeenRated, Is.EqualTo(true));
        }
    }
}
