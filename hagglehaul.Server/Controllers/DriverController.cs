using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hagglehaul.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverProfileService _driverProfileService;
        private readonly IRiderProfileService _riderProfileService;
        private readonly ITripService _tripService;
        private readonly IBidService _bidService;

        public DriverController(IDriverProfileService driverProfileService, IRiderProfileService riderProfileService, ITripService tripService, IBidService bidService)
        {
            _driverProfileService = driverProfileService;
            _riderProfileService = riderProfileService;
            _tripService = tripService;
            _bidService = bidService;
        }
    }
}
