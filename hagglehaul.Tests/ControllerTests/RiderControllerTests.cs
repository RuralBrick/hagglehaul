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

        private RiderController _controller;

        [OneTimeSetUp]
        public void DriverControllerTestsSetup()
        {
            _mockRiderProfileService = new Mock<IRiderProfileService>();
            _mockDriverProfileService = new Mock<IDriverProfileService>();
            _mockUserCoreService = new Mock<IUserCoreService>();
            _mockTripService = new Mock<ITripService>();
            _mockBidService = new Mock<IBidService>();

            _controller = new RiderController(
                _mockRiderProfileService.Object,
                _mockDriverProfileService.Object,
                _mockUserCoreService.Object,
                _mockTripService.Object,
                _mockBidService.Object
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
        public async Task RiderGetTripsTest()
        {
            _mockTripService.Setup(
                x => x.GetTripByIdAsync(It.IsAny<string>())
            )!.ReturnsAsync(
                (string s) => HhTestUtilities.GetTripData().FirstOrDefault(trip => trip.Id == s)
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "rider@example.com"),
                new Claim(ClaimTypes.Role, "rider")
            }, "mock"));

            var request = new CreateTrip
            {
                StartTime = DateTime.Now,
                PickupLat = 34.050,
                PickupLong = -118.250,
                DestinationLat = 40.731,
                DestinationLong = -73.935,
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            Assert.That(await _controller.PostRiderTrip(request), Is.InstanceOf<OkResult>());

            _mockTripService.Verify(x => x.CreateAsync(It.IsAny<Trip>()), Times.Once());

            var result = await _controller.GetAllRiderTrips() as OkObjectResult;
            var riderTrips = result.Value as List<RiderTripInfo>;
            var getTrip = riderTrips.FirstOrDefault();
            Assert.That(getTrip.StartTime, Is.EqualTo(request.StartTime));
            Assert.That(getTrip.PickupLat, Is.EqualTo(request.PickupLat));
            Assert.That(getTrip.PickupLong, Is.EqualTo(request.PickupLong));
            Assert.That(getTrip.DestinationLat, Is.EqualTo(request.DestinationLat));
            Assert.That(getTrip.DestinationLong, Is.EqualTo(request.DestinationLong));
        }
    }
}
