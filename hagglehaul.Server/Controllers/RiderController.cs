using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hagglehaul.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiderController : ControllerBase
    {
        private readonly IRiderProfileService _riderProfileService;
        private readonly IDriverProfileService _driverProfileService;
        private readonly ITripService _tripService;
        private readonly IBidService _bidService;

        public RiderController(IRiderProfileService riderProfileService, IDriverProfileService driverProfileService, ITripService tripService, IBidService bidService)
        {
            _riderProfileService = riderProfileService;
            _driverProfileService = driverProfileService;
            _tripService = tripService;
            _bidService = bidService;
        }
    }
}
