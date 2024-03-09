using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hagglehaul.Tests.ServiceTests
{
    internal class DriverProfileServiceTests : ServiceTestsBase
    {
        [SetUp]
        public void Setup()
        {
            _database.CreateCollection("DriverProfile");
        }

        [TearDown]
        public void TearDown()
        {
            _database.DropCollection("DriverProfile");
        }

        [Test]
        public async Task CanCreateAndDeleteDriverProfile()
        {
            var driverProfileService = new DriverProfileService(_database);
            var driverProfile = HhTestUtilities.GetDriverProfileData(1).First();

            Assert.DoesNotThrowAsync(async () => await driverProfileService.CreateAsync(driverProfile));
            var actual = await driverProfileService.GetAsync(driverProfile.Email);
            Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetDriverProfileData(1).First(), actual));
        }
        
        [Test]
        public async Task CanCreateAndUpdateDriverProfile()
        {
            var driverProfileService = new DriverProfileService(_database);
            var driverProfile = HhTestUtilities.GetDriverProfileData(1).First();

            Assert.DoesNotThrowAsync(async () => await driverProfileService.CreateAsync(driverProfile));
            var actual = await driverProfileService.GetAsync(driverProfile.Email);
            Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetDriverProfileData(1).First(), actual));

            driverProfile.Rating = 1.1;
            Assert.DoesNotThrowAsync(async () => await driverProfileService.UpdateAsync(driverProfile.Email, driverProfile));
            actual = await driverProfileService.GetAsync(driverProfile.Email);
            Assert.IsTrue(HhTestUtilities.CompareJson(driverProfile, actual));
        }
    }
}
