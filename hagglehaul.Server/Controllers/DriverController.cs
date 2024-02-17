using System.Security.Claims;
using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
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
        
        [HttpPost]
        [Route("createBid")]
        [Authorize]
        public async Task<IActionResult> CreateBid([FromBody] CreateBid request)
        {
            ClaimsPrincipal currentUser = this.User;
            var username = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            if (role != "driver")
            {
                return Unauthorized();
            }
            
            if (String.IsNullOrEmpty(request.TripId) || (await _tripService.GetTripByIdAsync(request.TripId)) == null)
            {
                return BadRequest(new { Error = "Invalid tripId" });
            }
            
            if (request.CentsAmount <= 0)
            {
                return BadRequest(new { Error = "Invalid centsAmount for trip" });
            }
            
            if ((await _bidService.GetDriverBidsAsync(username)).Any(bid => bid.TripId == request.TripId))
            {
                return BadRequest(new { Error = "Driver already bid on this trip" });
            }
            
            await _bidService.CreateAsync(new Bid
            {
                DriverEmail = username,
                TripId = request.TripId,
                CentsAmount = request.CentsAmount
            });
            
            return Ok();
        }
    }
}
