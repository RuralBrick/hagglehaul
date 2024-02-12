using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hagglehaul.Tests.ServiceTests
{
    internal class RiderProfileServiceTests : ServiceTestsBase
    {
        [SetUp]
        public void Setup()
        {
            _database.CreateCollection("RiderProfile");
        }

        [TearDown]
        public void TearDown()
        {
            _database.DropCollection("RiderProfile");
        }

        [Test]
        public async Task CanCreateAndDeleteRiderProfile()
        {
            var riderProfileService = new RiderProfileService(_database);
            var riderProfile = HhTestUtilities.GetRiderProfileData(1).First();

            Assert.DoesNotThrowAsync(async () => await riderProfileService.CreateAsync(riderProfile));
            var actual = await riderProfileService.GetAsync(riderProfile.Email);
            Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetRiderProfileData(1).First(), actual));
        }
    }
}
