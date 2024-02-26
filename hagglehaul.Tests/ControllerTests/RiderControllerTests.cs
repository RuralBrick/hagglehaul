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
    }
}
