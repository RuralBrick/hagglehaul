using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using hagglehaul.Server.Models;

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
        private readonly IGeographicRouteService _geographicRouteService;

        public RiderController(IRiderProfileService riderProfileService, IDriverProfileService driverProfileService, IUserCoreService userCoreService, ITripService tripService, IBidService bidService, IGeographicRouteService geographicRouteService)
        {
            _riderProfileService = riderProfileService;
            _driverProfileService = driverProfileService;
            _userCoreService = userCoreService;
            _tripService = tripService;
            _bidService = bidService;
            _geographicRouteService = geographicRouteService;
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
            foreach (Trip trip in allTrips)
            {
                //if trip date is in past, trip is archived
                //if trip date is in future, email null, trip is in bidding
                //if trip date is in future, email !null, trip is confirmed
                TimeSpan day = new TimeSpan(24, 0, 0);
                DateTime pastPlusOne = trip.StartTime.Add(day);
                //archived Trips
                GeographicRoute geographicRoute = await _geographicRouteService.GetGeographicRoute(trip.PickupLong, trip.PickupLat, trip.DestinationLong, trip.DestinationLat);
                uint cost = 0;
                if (trip.DriverEmail != null )
                {
                    List<Bid> bids = await _bidService.GetDriverBidsAsync(trip.DriverEmail);
                    foreach (Bid bid in bids)
                    {
                        if (bid.TripId == trip.Id)
                        {
                            cost = bid.CentsAmount; break;
                        }
                    }
                }
                bool hasDriver = !String.IsNullOrEmpty(trip.DriverEmail);
                if ( DateTime.Now > pastPlusOne )
                {
                    ArchivedRiderTrip archive = new ArchivedRiderTrip();
                    archive.TripId = trip.Id;
                    archive.TripName = trip.Name;
                    archive.StartTime = trip.StartTime;
                    archive.Thumbnail = geographicRoute.Image;
                    archive.Distance = geographicRoute.Distance;
                    archive.Duration = geographicRoute.Duration;
                    archive.HasDriver = hasDriver;
                    if (archive.HasDriver)
                    {
                        archive.Cost = cost;
                        DriverProfile driver = await _driverProfileService.GetAsync(trip.DriverEmail);
                        UserCore driverCore = await _userCoreService.GetAsync(trip.DriverEmail);
                        archive.DriverName = driverCore.Name;
                        archive.DriverNumRating = driver.NumRatings;
                        archive.DriverRating = driver.Rating;
                    }
                    archive.PickupAddress = trip.PickupAddress;
                    archive.DestinationAddress = trip.DestinationAddress;
                    archivedTrips.Add(archive);
                } else if (hasDriver)
                {
                    //confirmed trip
                    ConfirmedRiderTrip confirmedTrip = new ConfirmedRiderTrip();
                    DriverProfile driver = await _driverProfileService.GetAsync(trip.DriverEmail);
                    UserCore driverCore = await _userCoreService.GetAsync(trip.DriverEmail);
                    confirmedTrip.TripID = trip.Id;
                    confirmedTrip.TripName = trip.Name;
                    confirmedTrip.Thumbnail = geographicRoute.Image;
                    confirmedTrip.StartTime = trip.StartTime;
                    confirmedTrip.Distance = geographicRoute.Distance;
                    confirmedTrip.Duration = geographicRoute.Duration;
                    confirmedTrip.Cost = cost;
                    confirmedTrip.DriverName = driverCore.Name;
                    confirmedTrip.DriverRating = driver.Rating;
                    confirmedTrip.DriverNumRating = driver.NumRatings;
                    confirmedTrip.ShowRatingPrompt = trip.DriverHasBeenRated && DateTime.Now > trip.StartTime && DateTime.Now <= pastPlusOne;
                    confirmedTrip.DriverEmail = driverCore.Email;
                    confirmedTrip.DriverPhone = driverCore.Phone;
                    confirmedTrip.DriverCarModel = driver.CarDescription;
                    confirmedTrip.PickupAddress = trip.PickupAddress;
                    confirmedTrip.DestinationAddress = trip.DestinationAddress;
                    confirmedTrips.Add(confirmedTrip);
                }
                else
                {
                    //unconfirmed trip
                    UnconfirmedRiderTrip unconfirmedTrip = new UnconfirmedRiderTrip();
                    unconfirmedTrip.TripID = trip.Id;
                    unconfirmedTrip.TripName = trip.Name;
                    unconfirmedTrip.Thumbnail = geographicRoute.Image;
                    unconfirmedTrip.StartTime = trip.StartTime;
                    unconfirmedTrip.Distance = geographicRoute.Distance;
                    unconfirmedTrip.Duration = geographicRoute.Duration;
                    unconfirmedTrip.Bids = new List<BidUserView>();
                    List<Bid> tripBids = await _bidService.GetTripBidsAsync(trip.Id);
                    foreach (Bid tripBid in tripBids)
                    {
                        BidUserView bidUserView = new BidUserView();
                        UserCore driverCore = await _userCoreService.GetAsync(tripBid.DriverEmail);
                        DriverProfile driver = await _driverProfileService.GetAsync(tripBid.DriverEmail);
                        bidUserView.BidId = tripBid.Id;
                        bidUserView.YourBid = false;
                        bidUserView.DriverName = driverCore.Name;
                        bidUserView.DriverRating = driver.Rating;
                        bidUserView.DriverNumRating = driver.NumRatings;
                        bidUserView.Cost = tripBid.CentsAmount;
                        unconfirmedTrip.Bids.Add(bidUserView);
                    }
                    unconfirmedTrip.PickupAddress = trip.PickupAddress;
                    unconfirmedTrip.DestinationAddress = trip.DestinationAddress;
                    unconfirmedTrips.Add(unconfirmedTrip);
                }
            }
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
    }
}
