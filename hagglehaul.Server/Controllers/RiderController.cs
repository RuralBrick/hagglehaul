using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using hagglehaul.Server.Models;
using Swashbuckle.AspNetCore.Annotations;
using hagglehaul.Server.EmailViews;

namespace hagglehaul.Server.Controllers
{
    /// <summary>
    /// Controller for rider-related operations.
    /// </summary>
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
        private readonly IEmailNotificationService _emailNotificationService;

        public RiderController(
            IRiderProfileService riderProfileService,
            IDriverProfileService driverProfileService,
            IUserCoreService userCoreService,
            ITripService tripService,
            IBidService bidService,
            IGeographicRouteService geographicRouteService,
            IEmailNotificationService emailNotificationService
        )
        {
            _riderProfileService = riderProfileService;
            _driverProfileService = driverProfileService;
            _userCoreService = userCoreService;
            _tripService = tripService;
            _bidService = bidService;
            _geographicRouteService = geographicRouteService;
            _emailNotificationService = emailNotificationService;
        }

        /// <summary>
        /// Get the basic info of the current rider.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> with the rider's basic info,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid,
        /// <see cref="UnauthorizedResult"/> if the user is not a rider
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("about")]
        [ProducesResponseType(typeof(RiderBasicInfo), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get the basic info of the current rider.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Got the rider's basic info.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a rider.")]
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

        /// <summary>
        /// Gets the necessary info for a rider dashboard. Shows confirmed trips, trips in bidding, and past trips.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> with the rider's dashboard,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid,
        /// <see cref="UnauthorizedResult"/> if the user is not a rider
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("dashboard")]
        [SwaggerOperation(Summary = "Gets the necessary info for a rider dashboard. Shows confirmed trips, trips in bidding, and past trips.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Succesfully returned the dashboard.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid User or Authentication")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a rider.")]
        [ProducesResponseType(typeof(RiderDashboard), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            ClaimsPrincipal currentUser = this.User;
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            if (role != "rider")
            {
                return Unauthorized();
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
                DateTime pastPlusOne = trip.StartTime.ToLocalTime().Add(day);
                //archived Trips
                GeographicRoute geographicRoute = await _geographicRouteService.GetGeographicRoute(trip.PickupLong, trip.PickupLat, trip.DestinationLong, trip.DestinationLat);
                uint cost = 0;
                if (trip.DriverEmail != null )
                {
                    List<Bid> bids = await _bidService.GetDriverBidsAsync(trip.DriverEmail);
                    foreach (Bid bid in bids ?? Enumerable.Empty<Bid>())
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
                    archive.GeoJson = geographicRoute.GeoJson;
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
                    confirmedTrip.GeoJson = geographicRoute.GeoJson;
                    confirmedTrip.StartTime = trip.StartTime;
                    confirmedTrip.Distance = geographicRoute.Distance;
                    confirmedTrip.Duration = geographicRoute.Duration;
                    confirmedTrip.Cost = cost;
                    confirmedTrip.DriverName = driverCore.Name;
                    confirmedTrip.DriverRating = driver.Rating;
                    confirmedTrip.DriverNumRating = driver.NumRatings;
                    confirmedTrip.ShowRatingPrompt = !trip.DriverHasBeenRated && DateTime.Now > trip.StartTime.ToLocalTime() && DateTime.Now <= pastPlusOne;
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
                    unconfirmedTrip.GeoJson = geographicRoute.GeoJson;
                    unconfirmedTrip.StartTime = trip.StartTime;
                    unconfirmedTrip.Distance = geographicRoute.Distance;
                    unconfirmedTrip.Duration = geographicRoute.Duration;
                    unconfirmedTrip.Bids = new List<BidUserView>();
                    List<Bid> tripBids = await _bidService.GetTripBidsAsync(trip.Id);
                    foreach (Bid tripBid in tripBids ?? Enumerable.Empty<Bid>())
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

        /// <summary>
        /// Modify account details, including password.
        /// </summary>
        /// <param name="riderUpdate">The update form.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the account details are successfully updated,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid or there is an error with updating the password,
        /// <see cref="UnauthorizedResult"/> if the user is not a rider
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("modifyAcc")]
        [SwaggerOperation(Summary = "Modify account details, including password.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully updated the account details.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth or error with updating password.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a rider.")]
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
            if (role != "driver")
            {
                return Unauthorized();
            }
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

        /// <summary>
        /// Create a new trip.
        /// </summary>
        /// <param name="tripDetails">The form with the requested trip's details.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the trip is successfully created,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid, the party size is invalid, or the start time is in the past,
        /// <see cref="UnauthorizedResult"/> if the user is not a rider
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("trip")]
        [SwaggerOperation(Summary = "Create a new trip.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully created the trip.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth, invalid party size, or start time is in the past.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a rider.")]
        public async Task<IActionResult> PostRiderTrip([FromBody] CreateTrip tripDetails)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var email = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (role != "rider") { return Unauthorized(); };

            if (tripDetails.StartTime.ToLocalTime() < DateTime.Now) { return BadRequest(new { Error = "Start time is in the past" }); };

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

        /// <summary>
        /// Delete a trip.
        /// </summary>
        /// <param name="tripId">The ID of the trip to delete. Rider must have create this trip.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the trip is successfully deleted,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid, the trip does not exist, the trip has a driver, or the trip has already started,
        /// <see cref="UnauthorizedResult"/> if the user is not a rider
        /// </returns>
        [Authorize]
        [HttpDelete]
        [Route("trip")]
        [SwaggerOperation(Summary = "Delete a trip.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully deleted the trip.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth, trip does not exist, trip has a driver, or trip has already started.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a rider.")]
        public async Task<IActionResult> DeleteRiderTrip([FromQuery] string tripId)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var email = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (role != "rider") { return Unauthorized(); }

            Trip trip = await _tripService.GetTripByIdAsync(tripId);
            if (trip == null) { return BadRequest(new { Error = "Trip does not exist" }); }
            if (trip.RiderEmail != email) { return Unauthorized(); }
            if (!string.IsNullOrEmpty(trip.DriverEmail)) { return BadRequest(new { Error = "Trip has a driver" }); }
            if (trip.StartTime.ToLocalTime() < DateTime.Now) { return BadRequest(new { Error = "Trip has already started" }); }

            await _tripService.DeleteAsync(tripId);
            await _bidService.DeleteByTripIdAsync(tripId);
            return Ok();
        }

        /// <summary>
        /// Confirm a driver for a trip.
        /// </summary>
        /// <param name="addTripDriver">The form to confirm a driver with.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the driver is successfully confirmed,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid, the trip does not exist, the trip has a driver, the trip has already started, or the bid does not exist,
        /// <see cref="UnauthorizedResult"/> if the user is not a rider
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("tripDriver")]
        [SwaggerOperation(Summary = "Confirm a driver for a trip.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully confirmed the driver.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth, trip does not exist, trip has a driver, trip has already started, or bid does not exist.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a rider.")]
        public async Task<IActionResult> ConfirmDriver([FromBody] AddTripDriver addTripDriver)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var email = currentUser.FindFirstValue(ClaimTypes.Name);
            var role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (role != "rider") { return Unauthorized(); }

            Trip trip = await _tripService.GetTripByIdAsync(addTripDriver.TripId);
            if (trip == null) { return BadRequest(new { Error = "Trip does not exist" }); }
            if (trip.RiderEmail != email) { return Unauthorized(); }
            if (!string.IsNullOrEmpty(trip.DriverEmail)) { return BadRequest(new { Error = "Trip already has a driver" }); }
            if (trip.StartTime.ToLocalTime() < DateTime.Now) { return BadRequest(new { Error = "Trip is in the past and is therefore cancelled" }); }

            var bids = await _bidService.GetTripBidsAsync(addTripDriver.TripId);
            Bid bid = bids.FirstOrDefault(b => b.Id == addTripDriver.BidId);
            if (bid == null) { return BadRequest(new { Error = "Bid does not exist" }); }
            trip.DriverEmail = bid.DriverEmail;
            await _tripService.UpdateAsync(addTripDriver.TripId, trip);

            UserCore riderUser = await _userCoreService.GetAsync(email);
            UserCore driverUser = await _userCoreService.GetAsync(trip.DriverEmail);
            DriverProfile driverProfile = await _driverProfileService.GetAsync(trip.DriverEmail);
            _ = _emailNotificationService.SendEmailNotification(
                EmailNotificationType.Confirmation,
                email,
                new ConfirmationEmail
                {
                    RiderName = riderUser.Name,
                    TripName = trip.Name,
                    DriverName = driverUser.Name,
                    DriverRating = driverProfile.Rating,
                    Price = (decimal)(bid.CentsAmount / 100.0),
                    DriverPhone = driverUser.Phone,
                    DriverEmail = driverUser.Email,
                    StartTime = trip.StartTime,
                    PickupAddress = trip.PickupAddress,
                    DestinationAddress = trip.DestinationAddress,
                    RiderEmail = email,
                }
            );
            _ = _emailNotificationService.SendEmailNotification(
                EmailNotificationType.AcceptedBid,
                trip.DriverEmail,
                new AcceptedBidEmail
                {
                    DriverName = driverUser.Name,
                    TripName = trip.Name,
                    Price = (decimal)(bid.CentsAmount / 100.0),
                    RiderName = riderUser.Name,
                    RiderPhone = riderUser.Phone,
                    RiderEmail = riderUser.Email,
                    StartTime = trip.StartTime,
                    PickupAddress = trip.PickupAddress,
                    DestinationAddress = trip.DestinationAddress,
                    DriverEmail = trip.DriverEmail,
                }
            );

            return Ok();
        }

        /// <summary>
        /// Rate a driver.
        /// </summary>
        /// <param name="giveRating">The form to rate a user.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the driver is successfully rated,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid, the trip does not exist, the trip has no driver, the trip has not been taken yet, or the driver has already been rated,
        /// <see cref="UnauthorizedResult"/> if the user is not a rider
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("rating")]
        [SwaggerOperation(Summary = "Rate a driver.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully rated the driver.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth, trip does not exist, trip has no driver, trip has not been taken yet, or driver has already been rated.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a rider.")]
        public async Task<IActionResult> RateDriver([FromBody] GiveRating giveRating)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (role != "rider") { return Unauthorized(); }

            var trip = await _tripService.GetTripByIdAsync(giveRating.TripId);
            if (trip == null) { return BadRequest(new { Error = "Trip does not exist" }); }

            var riderEmail = currentUser.FindFirstValue(ClaimTypes.Name);
            if (riderEmail != trip.RiderEmail) { return Unauthorized(); }

            var driverEmail = trip.DriverEmail;
            if (string.IsNullOrEmpty(driverEmail)) { return BadRequest(new { Error = "Trip does not have a driver" }); }

            Console.WriteLine("Current time: " + DateTime.Now);
            Console.WriteLine("Trip start time: " + trip.StartTime);
            if (trip.StartTime.ToLocalTime() >= DateTime.Now) { return BadRequest(new { Error = "Trip has not been taken yet" }); }

            if (trip.DriverHasBeenRated) { return BadRequest(new { Error = "Driver has already been rated for this trip" }); }

            var driver = await _driverProfileService.GetAsync(driverEmail);
            if (driver == null) { return BadRequest(new { Error = "Driver does not exist" }); }

            if (driver.Rating == null)
                driver.Rating = 0;
            if (driver.NumRatings == null)
                driver.NumRatings = 0;

            var totalRatings = driver.Rating * (double)driver.NumRatings;

            driver.NumRatings++;

            driver.Rating = (totalRatings + (double)giveRating.RatingGiven) / (double)driver.NumRatings;

            await _driverProfileService.UpdateAsync(driver.Email, driver);

            trip.DriverHasBeenRated = true;

            await _tripService.UpdateAsync(giveRating.TripId, trip);
            return Ok();
        }
    }
}
