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
        private readonly IUserCoreService _userCoreService;
        private readonly ITripService _tripService;
        private readonly IBidService _bidService;

        public DriverController(IDriverProfileService driverProfileService, IRiderProfileService riderProfileService, IUserCoreService userCoreService, ITripService tripService, IBidService bidService)
        {
            _driverProfileService = driverProfileService;
            _riderProfileService = riderProfileService;
            _userCoreService = userCoreService;
            _tripService = tripService;
            _bidService = bidService;
        }

        [Authorize]
        [HttpGet]
        [Route("about")]
        [ProducesResponseType(typeof(DriverBasicInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ClaimsPrincipal currentUser = this.User;
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email
            UserCore userCore = await _userCoreService.GetAsync(email);
            DriverProfile driverProfile = await _driverProfileService.GetAsync(email);
            DriverBasicInfo driverBasicInfo = new DriverBasicInfo();
            driverBasicInfo.Name = userCore.Name;
            driverBasicInfo.Email = email;
            driverBasicInfo.Phone = userCore.Phone;
            driverBasicInfo.CarDescription = driverProfile.CarDescription;
            return Ok(driverBasicInfo);
        }

        [Authorize]
        [HttpPost]
        [Route("modifyAcc")]
        public async Task<IActionResult> ModifyAccountDetails([FromBody] DriverUpdate driverUpdate)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            bool changingPassword = !String.IsNullOrEmpty(driverUpdate.NewPassword);
            if (String.IsNullOrEmpty(driverUpdate.CurrentPassword) && changingPassword)
            {
                return BadRequest(new { Error = "Can't make a new password" });
            }

            //check role for error
            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email

            UserCore userCore = await _userCoreService.GetAsync(email);
            DriverProfile driverProfile = await _driverProfileService.GetAsync(email);
            if (changingPassword)
            {

                if (!_userCoreService.ComparePasswordToHash(driverUpdate.CurrentPassword, userCore.PasswordHash, userCore.Salt))
                {
                    return BadRequest(new { Error = "Current Password is invalid" });
                }

            }

            if (changingPassword)
            {
                Console.WriteLine("changing pass");
                _userCoreService.CreatePasswordHash(driverUpdate.NewPassword, out var newHash, out var newSalt);
                userCore.PasswordHash = newHash;
                userCore.Salt = newSalt;
            }
            if (!String.IsNullOrEmpty(driverUpdate.Name))
            {
                userCore.Name = driverUpdate.Name;
            }
            if (!String.IsNullOrEmpty(driverUpdate.Phone))
            {
                userCore.Phone = driverUpdate.Phone;
            }
            if (!String.IsNullOrEmpty(driverUpdate.CarDescription))
            {
                driverProfile.CarDescription = driverUpdate.CarDescription;
            }
            await _userCoreService.UpdateAsync(email, userCore);
            await _driverProfileService.UpdateAsync(email, driverProfile);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("bid")]
        public async Task<IActionResult> GetDriverBids()
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var email = currentUser.FindFirstValue(ClaimTypes.Name);

            var dbBids = await _bidService.GetDriverBidsAsync(email);
            var driverBids = dbBids.Select(bid => new BidInfo
            {
                DriverEmail = bid.DriverEmail,
                TripId = bid.TripId,
                CentsAmount = bid.CentsAmount,
            }).ToList();
            return Ok(driverBids);
        }

        [HttpPost]
        [HttpPatch]
        [Route("bid")]
        [Authorize]
        public async Task<IActionResult> CreateOrUpdateBid([FromBody] CreateOrUpdateBid request)
        {
            ClaimsPrincipal currentUser = this.User;
            var username = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            if (role != "driver")
            {
                return Unauthorized();
            }
            
            Trip trip = await _tripService.GetTripByIdAsync(request.TripId);
            if (String.IsNullOrEmpty(request.TripId) || trip == null)
            {
                return BadRequest(new { Error = "Invalid tripId" });
            }
            
            if (!String.IsNullOrEmpty(trip.DriverEmail) || trip.StartTime < DateTime.Now)
            {
                return BadRequest(new { Error = "The trip is either confirmed or in the past" });
            }
            
            if (request.CentsAmount <= 0)
            {
                return BadRequest(new { Error = "Invalid centsAmount for trip" });
            }

            Bid existingBid = (await _bidService.GetDriverBidsAsync(username)).FirstOrDefault(bid => bid.TripId == request.TripId);
            if (existingBid != null)
            {
                // Create bid
                existingBid.CentsAmount = request.CentsAmount;
                await _bidService.UpdateAsync(existingBid.Id, existingBid);
            }
            else
            {
                // Update bid
                await _bidService.CreateAsync(new Bid
                {
                    DriverEmail = username,
                    TripId = request.TripId,
                    CentsAmount = request.CentsAmount
                });
            }
            
            return Ok();
        }
        
        [HttpDelete]
        [Route("bid")]
        [Authorize]
        public async Task<IActionResult> DeleteBid([FromQuery] string tripId)
        {
            ClaimsPrincipal currentUser = this.User;
            var username = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            if (role != "driver")
            {
                return Unauthorized();
            }
        
            Trip trip = await _tripService.GetTripByIdAsync(tripId);
            if (String.IsNullOrEmpty(tripId) || trip == null)
            {
                return BadRequest(new { Error = "Invalid tripId" });
            }
        
            if (!String.IsNullOrEmpty(trip.DriverEmail) || trip.StartTime < DateTime.Now)
            {
                return BadRequest(new { Error = "The trip is either confirmed or in the past" });
            }
        
            Bid existingBid = (await _bidService.GetDriverBidsAsync(username)).FirstOrDefault(bid => bid.TripId == tripId);
            if (existingBid != null)
            {
                await _bidService.DeleteAsync(existingBid.Id);
            }
            else
            {
                return BadRequest(new { Error = "No bid found for this trip" });
            }
            
            return Ok();
        }

        [HttpGet]
        [Route("tripBids")]
        public async Task<IActionResult> GetTripBids([FromQuery] string tripId)
        {
            var dbBids = await _bidService.GetTripBidsAsync(tripId);
            var driverBids = dbBids.Select(bid => new BidInfo
            {
                DriverEmail = bid.DriverEmail,
                TripId = bid.TripId,
                CentsAmount = bid.CentsAmount,
            }).ToList();
            return Ok(driverBids);
        }
    }
}
