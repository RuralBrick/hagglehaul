using System.Runtime.InteropServices;
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
        private readonly IGeographicRouteService _geographicRouteService;

        public DriverController(IDriverProfileService driverProfileService, IRiderProfileService riderProfileService, IUserCoreService userCoreService, ITripService tripService, IBidService bidService, IGeographicRouteService geographicRouteService)
        {
            _driverProfileService = driverProfileService;
            _riderProfileService = riderProfileService;
            _userCoreService = userCoreService;
            _tripService = tripService;
            _bidService = bidService;
            _geographicRouteService = geographicRouteService;
        }

        [Authorize]
        [HttpGet]
        [Route("dashboard")]
        [ProducesResponseType(typeof(DriverDashboard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            ClaimsPrincipal currentUser = this.User;
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email
            UserCore userCore = await _userCoreService.GetAsync(email);
            DriverDashboard driverDashboard = new DriverDashboard();
            List<ConfirmedDriverTrip> confirmedTrips = new List<ConfirmedDriverTrip>();
            List<UnconfirmedDriverTrip> unconfirmedTrips = new List<UnconfirmedDriverTrip>();
            List<ArchivedDriverTrip> archivedTrips = new List<ArchivedDriverTrip>();
            List<Bid> allBids = await _bidService.GetDriverBidsAsync(email);
            foreach (Bid bid in allBids)
            {
                //if trip date is in past, trip is archived
                //if trip date is in future, email null, trip is in bidding
                //if trip date is in future, email !null, trip is confirmed
                Trip trip = await _tripService.GetTripByIdAsync(bid.TripId);
                TimeSpan day = new TimeSpan(24, 0, 0);
                DateTime pastPlusOne = trip.StartTime.Add(day);
                //archived Trips
                GeographicRoute geographicRoute = await _geographicRouteService.GetGeographicRoute(trip.PickupLong, trip.PickupLat, trip.DestinationLong, trip.DestinationLat);
                uint cost = bid.CentsAmount;
                bool hasDriver = !String.IsNullOrEmpty(trip.DriverEmail);
                if (DateTime.Now > pastPlusOne || (hasDriver && trip.DriverEmail != email) )
                {
                    ArchivedDriverTrip archive = new ArchivedDriverTrip();
                    archive.TripId = trip.Id;
                    archive.TripName = trip.Name;
                    archive.StartTime = trip.StartTime;
                    archive.Thumbnail = geographicRoute.Image;
                    archive.Distance = geographicRoute.Distance;
                    archive.Duration = geographicRoute.Duration;
                    archive.Cost = cost;
                    DriverProfile rider = await _driverProfileService.GetAsync(trip.RiderEmail);
                    UserCore riderCore = await _userCoreService.GetAsync(trip.RiderEmail);
                    archive.RiderName = riderCore.Name;
                    archive.RiderNumRating = rider.NumRatings;
                    archive.RiderRating = rider.Rating;
                    archive.PickupAddress = trip.PickupAddress;
                    archive.DestinationAddress = trip.DestinationAddress;
                    archivedTrips.Add(archive);
                }
                else if (hasDriver && trip.DriverEmail == email)
                {
                    //confirmed trip
                    ConfirmedDriverTrip confirmedTrip = new ConfirmedDriverTrip();
                    RiderProfile rider = await _riderProfileService.GetAsync(trip.RiderEmail);
                    UserCore riderCore = await _userCoreService.GetAsync(trip.RiderEmail);
                    confirmedTrip.TripID = trip.Id;
                    confirmedTrip.TripName = trip.Name;
                    confirmedTrip.Thumbnail = geographicRoute.Image;
                    confirmedTrip.StartTime = trip.StartTime;
                    confirmedTrip.Distance = geographicRoute.Distance;
                    confirmedTrip.Duration = geographicRoute.Duration;
                    confirmedTrip.Cost = cost;
                    confirmedTrip.RiderName = riderCore.Name;
                    confirmedTrip.RiderRating = rider.Rating;
                    confirmedTrip.RiderNumRating = rider.NumRatings;
                    confirmedTrip.ShowRatingPrompt = trip.DriverHasBeenRated && DateTime.Now > trip.StartTime && DateTime.Now <= pastPlusOne;
                    confirmedTrip.RiderEmail = riderCore.Email;
                    confirmedTrip.RiderPhone = riderCore.Phone;
                    confirmedTrip.PickupAddress = trip.PickupAddress;
                    confirmedTrip.DestinationAddress = trip.DestinationAddress;
                    confirmedTrips.Add(confirmedTrip);
                }
                else
                {
                    //unconfirmed trip
                    UnconfirmedDriverTrip unconfirmedTrip = new UnconfirmedDriverTrip();
                    UserCore riderCore = await _userCoreService.GetAsync(trip.RiderEmail);
                    RiderProfile rider = await _riderProfileService.GetAsync(trip.RiderEmail);
                    unconfirmedTrip.TripID = trip.Id;
                    unconfirmedTrip.TripName = trip.Name;
                    unconfirmedTrip.Thumbnail = geographicRoute.Image;
                    unconfirmedTrip.StartTime = trip.StartTime;
                    unconfirmedTrip.Distance = geographicRoute.Distance;
                    unconfirmedTrip.Duration = geographicRoute.Duration;
                    unconfirmedTrip.RiderName = riderCore.Name;
                    unconfirmedTrip.RiderRating = rider.Rating;
                    unconfirmedTrip.RiderNumRating = rider.NumRatings;
                    List<Bid> tripBids = await _bidService.GetTripBidsAsync(trip.Id);
                    for (int i = 0; i < tripBids.Count; i++)
                    {
                        var tripBid = tripBids[i];
                        if (tripBid.DriverEmail == email)
                        {
                            tripBids.RemoveAt(i);
                            tripBids.Insert(0, tripBid);
                            break;
                        }
                    }
                    unconfirmedTrip.Bids = tripBids;
                    unconfirmedTrip.PickupAddress = trip.PickupAddress;
                    unconfirmedTrip.DestinationAddress = trip.DestinationAddress;
                    unconfirmedTrips.Add(unconfirmedTrip);
                }
            }
            driverDashboard.ConfirmedTrips = confirmedTrips;
            driverDashboard.TripsInBidding = unconfirmedTrips;
            driverDashboard.ArchivedTrips = archivedTrips;
            return Ok(driverDashboard);
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

            var bids = await _bidService.GetDriverBidsAsync(email);
            return Ok(bids);
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

        [Authorize]
        [HttpGet]
        [Route("trip")]
        public async Task<IActionResult> GetDriverTrips()
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var email = currentUser.FindFirstValue(ClaimTypes.Name);

            var trips = await _tripService.GetDriverTripsAsync(email);
            return Ok(trips);
        }

        [HttpGet]
        [Route("allTrips")]
        public async Task<IActionResult> GetAllAvailableTrips()
        {
            var allTrips = await _tripService.GetAllTripsAsync();
            var availableTrips = allTrips.Where(trip => trip.DriverEmail == null);
            return Ok(availableTrips);
        }

        [HttpGet]
        [Route("tripMarket")]
        public async Task<IActionResult> GetAvailableTrips([FromBody] TripMarketOptions options)
        {
            var allTrips = await _tripService.GetAllTripsAsync();

            var filteredTrips = allTrips;

            IOrderedEnumerable<Trip> sortedTrips = null!;
            foreach (var sortMethod in options.SortMethods)
            {
                switch (sortMethod)
                {
                    case "euclideanDistance":
                        break;
                    case "routeDistance":
                        break;
                    case "currentToStartDistance":
                        break;
                    case "endToTargetDistance":
                        break;
                    case "currentMinBid":
                        break;
                    case "startTime":
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(trip => trip.StartTime);
                        else
                            sortedTrips = sortedTrips.ThenBy(trip => trip.StartTime);
                        break;
                    default:
                        return BadRequest(new { Error = "Invalid sort method" });
                }
            }

            List<Trip> finalTrips;
            if (sortedTrips != null)
                finalTrips = sortedTrips.ToList();
            else
                finalTrips = filteredTrips;

            return Ok(finalTrips);
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
