using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using hagglehaul.Server.Models;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using MongoDB.Bson.IO;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace hagglehaul.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiderController : ControllerBase
    {
        private readonly IRiderProfileService _riderProfileService;
        private readonly IDriverProfileService _driverProfileService;
        private readonly IUserCoreService _userCoreService;
        private readonly ITripService _tripService;
        private readonly IBidService _bidService;

        public RiderController(IRiderProfileService riderProfileService, IDriverProfileService driverProfileService, IUserCoreService userCoreService, ITripService tripService, IBidService bidService)
        {
            _riderProfileService = riderProfileService;
            _driverProfileService = driverProfileService;
            _userCoreService = userCoreService;
            _tripService = tripService;
            _bidService = bidService;
        }

        [Authorize]
        [HttpGet]
        [Route("about")]
        [ProducesResponseType(typeof(RiderBasicInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            ClaimsPrincipal currentUser = this.User;
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email
            UserCore userCore = await _userCoreService.GetAsync(email);
            RiderBasicInfo riderBasicInfo = new RiderBasicInfo();
            riderBasicInfo.Name = userCore.Name;
            riderBasicInfo.Email = email;
            riderBasicInfo.Phone = userCore.Phone;
            return Ok(riderBasicInfo);
        }

        [Authorize]
        [HttpGet]
        [Route("dashboard")]
        [ProducesResponseType(typeof(RiderDashboard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            ClaimsPrincipal currentUser = this.User;
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email
            UserCore userCore = await _userCoreService.GetAsync(email);
            RiderDashboard riderDashboard = new RiderDashboard();
            List<ConfirmedRiderTrip> confirmedTrips = new List<ConfirmedRiderTrip>();
            List<UnconfirmedRiderTrip> unconfirmedTrips = new List<UnconfirmedRiderTrip>();
            List<ArchivedRiderTrip> archivedTrips = new List<ArchivedRiderTrip>();
            List<Trip> allTrips = await _tripService.GetRiderTripsAsync(email);

            riderDashboard.ConfirmedTrips = confirmedTrips;
            riderDashboard.TripsInBidding = unconfirmedTrips;
            riderDashboard.ArchivedTrips = archivedTrips;
            return Ok(riderDashboard);
        }

        [Authorize]
        [HttpPost]
        [Route("modifyAcc")]
        public async Task<IActionResult> ModifyAccountDetails([FromBody] RiderUpdate riderUpdate )
        {
            ClaimsPrincipal currentUser = this.User;
  
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            bool changingPassword = !String.IsNullOrEmpty(riderUpdate.NewPassword);
            if (String.IsNullOrEmpty(riderUpdate.CurrentPassword) && changingPassword)
            {
                return BadRequest(new { Error = "Can't make a new password" });
            }
            
            //check role for error
            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            UserCore userCore = await _userCoreService.GetAsync(email);
            
            if (changingPassword)
            {

                if (!_userCoreService.ComparePasswordToHash(riderUpdate.CurrentPassword, userCore.PasswordHash,userCore.Salt))
                {
                    return BadRequest(new { Error = "Current Password is invalid" });
                }
                
            }
            
            if (changingPassword) {
                Console.WriteLine("changing pass");
                _userCoreService.CreatePasswordHash(riderUpdate.NewPassword, out var newHash, out var newSalt);
                userCore.PasswordHash = newHash;
                userCore.Salt = newSalt;
            }
            if (!String.IsNullOrEmpty(riderUpdate.Name))
            {
                userCore.Name = riderUpdate.Name;
            }
            if (!String.IsNullOrEmpty(riderUpdate.Phone))
            {
                userCore.Phone = riderUpdate.Phone;
            }
            await _userCoreService.UpdateAsync(email, userCore);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("trip")]
        public async Task<IActionResult> GetAllRiderTrips()
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var email = currentUser.FindFirstValue(ClaimTypes.Name);

            var trips = await _tripService.GetRiderTripsAsync(email);
            return Ok(trips);
        }

        [Authorize]
        [HttpPost]
        [Route("trip")]
        public async Task<IActionResult> PostRiderTrip([FromBody] CreateTrip tripDetails)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var email = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (role != "rider") { return Unauthorized(); };
            
            if (tripDetails.StartTime < DateTime.Now) { return BadRequest(new { Error = "Start time is in the past" }); };
            
            if (tripDetails.PartySize is <= 0 or > 10) { return BadRequest(new { Error = "Invalid party size" }); };

            Trip trip = new Trip
            {
                RiderEmail = email,
                Name = tripDetails.Name,
                StartTime = tripDetails.StartTime,
                PickupLong = tripDetails.PickupLong,
                PickupLat = tripDetails.PickupLat,
                DestinationLong = tripDetails.DestinationLong,
                DestinationLat = tripDetails.DestinationLat,
                PickupAddress = tripDetails.PickupAddress,
                DestinationAddress = tripDetails.DestinationAddress,
                PartySize = tripDetails.PartySize,
                RiderHasBeenRated = false,
                DriverHasBeenRated = false,
            };

            await _tripService.CreateAsync(trip);
            return Ok();
        }

        [HttpGet]
        [Route("tripBids")]
        public async Task<IActionResult> GetTripBids([FromQuery] string tripId)
        {
            var bids = await _bidService.GetTripBidsAsync(tripId);
            return Ok(bids);
        }
    }
}
