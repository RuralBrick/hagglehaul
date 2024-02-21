﻿using Azure.Core;
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
            var riderTripData = HhTestUtilities.GetTripData()
                                               .Where(trip => trip.RiderEmail == "rider@example.com")
                                               .ToList();
            _mockTripService.Setup(
                x => x.GetRiderTripsAsync(It.IsAny<string>())
            )!.ReturnsAsync(
                (string s) => riderTripData
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

            var result = await _controller.GetAllRiderTrips() as OkObjectResult;

            Assert.That(result, Is.TypeOf<OkObjectResult>());

            var riderTrips = result.Value as List<RiderTripInfo>;

            Assert.That(riderTrips, Is.TypeOf<List<RiderTripInfo>>());

            for (int i = 0; i < riderTrips.Count; i++)
            {
                Assert.That(riderTrips[i].StartTime, Is.EqualTo(riderTripData[i].StartTime));
                Assert.That(riderTrips[i].PickupLat, Is.EqualTo(riderTripData[i].PickupLat));
                Assert.That(riderTrips[i].PickupLong, Is.EqualTo(riderTripData[i].PickupLong));
                Assert.That(riderTrips[i].DestinationLat, Is.EqualTo(riderTripData[i].DestinationLat));
                Assert.That(riderTrips[i].DestinationLong, Is.EqualTo(riderTripData[i].DestinationLong));
            }
        }
    }
}
