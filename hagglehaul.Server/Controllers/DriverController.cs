﻿using System.Security.Claims;
using hagglehaul.Server.EmailViews;
using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Controllers
{
    /// <summary>
    /// Controller for driver-related operations.
    /// </summary>
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
        private readonly IEmailNotificationService _emailNotificationService;

        public DriverController(
            IDriverProfileService driverProfileService,
            IRiderProfileService riderProfileService,
            IUserCoreService userCoreService,
            ITripService tripService,
            IBidService bidService,
            IGeographicRouteService geographicRouteService,
            IEmailNotificationService emailNotificationService
        )
        {
            _driverProfileService = driverProfileService;
            _riderProfileService = riderProfileService;
            _userCoreService = userCoreService;
            _tripService = tripService;
            _bidService = bidService;
            _geographicRouteService = geographicRouteService;
            _emailNotificationService = emailNotificationService;
        }

        /// <summary>
        /// Gets the necessary info for a driver dashboard. Shows confirmed trips, trips in bidding, and past trips.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> if the dashboard is successfully returned,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid or not authenticated,
        /// <see cref="UnauthorizedResult"/> if the user is not a driver
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("dashboard")]
        [ProducesResponseType(typeof(DriverDashboard), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Gets the necessary info for a driver dashboard. Shows confirmed trips, trips in bidding, and past trips.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Succesfully returned the dashboard.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid User or Authentication")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a driver.")]
        public async Task<IActionResult> GetDashboard()
        {
            ClaimsPrincipal currentUser = this.User;
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            if (role != "driver")
            {
                return Unauthorized();
            }

            var email = currentUser.FindFirstValue(ClaimTypes.Name); //name is the email
            UserCore userCore = await _userCoreService.GetAsync(email);
            DriverDashboard driverDashboard = new DriverDashboard();
            List<ConfirmedDriverTrip> confirmedTrips = new List<ConfirmedDriverTrip>();
            List<UnconfirmedDriverTrip> unconfirmedTrips = new List<UnconfirmedDriverTrip>();
            List<ArchivedDriverTrip> archivedTrips = new List<ArchivedDriverTrip>();
            List<Bid> allBids = await _bidService.GetDriverBidsAsync(email);
            foreach (Bid bid in allBids ?? Enumerable.Empty<Bid>())
            {
                //if trip date is in past, trip is archived
                //if trip date is in future, email null, trip is in bidding
                //if trip date is in future, email !null, trip is confirmed
                Trip trip = await _tripService.GetTripByIdAsync(bid.TripId);
                TimeSpan day = new TimeSpan(24, 0, 0);
                DateTime pastPlusOne = trip.StartTime.Add(day).ToLocalTime();
                //archived Trips
                GeographicRoute geographicRoute = await _geographicRouteService.GetGeographicRoute(trip.PickupLong, trip.PickupLat, trip.DestinationLong, trip.DestinationLat);
                uint cost = bid.CentsAmount;
                bool hasDriver = !String.IsNullOrEmpty(trip.DriverEmail);
                if (DateTime.Now > pastPlusOne || (hasDriver && trip.DriverEmail != email))
                {
                    ArchivedDriverTrip archive = new ArchivedDriverTrip();
                    archive.TripId = trip.Id;
                    archive.TripName = trip.Name;
                    archive.StartTime = trip.StartTime;
                    archive.Thumbnail = geographicRoute.Image;
                    archive.GeoJson = geographicRoute.GeoJson;
                    archive.Distance = geographicRoute.Distance;
                    archive.Duration = geographicRoute.Duration;
                    archive.Cost = cost;
                    RiderProfile rider = await _riderProfileService.GetAsync(trip.RiderEmail);
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
                    confirmedTrip.GeoJson = geographicRoute.GeoJson;
                    confirmedTrip.StartTime = trip.StartTime;
                    confirmedTrip.Distance = geographicRoute.Distance;
                    confirmedTrip.Duration = geographicRoute.Duration;
                    confirmedTrip.Cost = cost;
                    confirmedTrip.RiderName = riderCore.Name;
                    confirmedTrip.RiderRating = rider.Rating;
                    confirmedTrip.RiderNumRating = rider.NumRatings;
                    confirmedTrip.ShowRatingPrompt = !trip.RiderHasBeenRated && DateTime.Now > trip.StartTime.ToLocalTime() && DateTime.Now <= pastPlusOne;
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
                    unconfirmedTrip.GeoJson = geographicRoute.GeoJson;
                    unconfirmedTrip.StartTime = trip.StartTime;
                    unconfirmedTrip.Distance = geographicRoute.Distance;
                    unconfirmedTrip.Duration = geographicRoute.Duration;
                    unconfirmedTrip.RiderName = riderCore.Name;
                    unconfirmedTrip.RiderRating = rider.Rating;
                    unconfirmedTrip.RiderNumRating = rider.NumRatings;
                    List<Bid> tripBids = await _bidService.GetTripBidsAsync(trip.Id);
                    unconfirmedTrip.Bids = new List<BidUserView>();
                    foreach (Bid tripBid in tripBids ?? Enumerable.Empty<Bid>())
                    {
                        BidUserView bidUserView = new BidUserView();
                        UserCore driverCore = await _userCoreService.GetAsync(tripBid.DriverEmail);
                        DriverProfile driver = await _driverProfileService.GetAsync(tripBid.DriverEmail);
                        bidUserView.BidId = tripBid.Id;
                        bidUserView.DriverName = driverCore.Name;
                        bidUserView.DriverRating = driver.Rating;
                        bidUserView.DriverNumRating = driver.NumRatings;
                        bidUserView.Cost = tripBid.CentsAmount;
                        if (tripBid.DriverEmail == email)
                        {
                            bidUserView.YourBid = true;
                            unconfirmedTrip.Bids.Insert(0, bidUserView);
                        }
                        else
                        {
                            bidUserView.YourBid = false;
                            unconfirmedTrip.Bids.Add(bidUserView);
                        }
                    }
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

        /// <summary>
        /// Get the basic info of the current driver.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> if the driver's basic info is successfully returned,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid or not authenticated,
        /// <see cref="UnauthorizedResult"/> if the user is not a driver
        /// </returns>
        [Authorize]
        [HttpGet]
        [Route("about")]
        [ProducesResponseType(typeof(DriverBasicInfo), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get the basic info of the current driver.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Got the driver's basic info.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a driver.")]
        public async Task<IActionResult> Get()
        {
            ClaimsPrincipal currentUser = this.User;
            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            if (role != "driver")
            {
                return Unauthorized();
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

        /// <summary>
        /// Modify account details, including password.
        /// </summary>
        /// <param name="driverUpdate">The update form.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the account details are successfully updated,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid or not authenticated,
        /// <see cref="UnauthorizedResult"/> if the user is not a driver
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("modifyAcc")]
        [SwaggerOperation(Summary = "Modify account details, including password.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully updated the account details.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth or error with updating password.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a driver.")]
        public async Task<IActionResult> ModifyAccountDetails([FromBody] DriverUpdate driverUpdate)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null)
            {
                return BadRequest(new { Error = "Invalid User/Auth" });
            }
            var role = currentUser.FindFirstValue(ClaimTypes.Role);
            if (role != "driver")
            {
                return Unauthorized();
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

        /// <summary>
        /// Create or update a bid for a trip.
        /// </summary>
        /// <param name="request">The bidding form.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the bid was successfully created or updated,
        /// <see cref="BadRequestObjectResult"/> if the tripId is invalid or the trip is either confirmed or in the past,
        /// <see cref="UnauthorizedResult"/> if the user is not a driver
        /// </returns>
        [HttpPost]
        [HttpPatch]
        [Route("bid")]
        [Authorize]
        [SwaggerOperation(Summary = "Create or update a bid for a trip.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The bid was successfully created or updated.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The tripId is invalid or the trip is either confirmed or in the past.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a driver.")]
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

            if (!String.IsNullOrEmpty(trip.DriverEmail) || trip.StartTime.ToLocalTime() < DateTime.Now)
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

            UserCore riderUser = await _userCoreService.GetAsync(trip.RiderEmail);
            UserCore driverUser = await _userCoreService.GetAsync(username);
            DriverProfile driverProfile = await _driverProfileService.GetAsync(username);
            _ = _emailNotificationService.SendEmailNotification(
                EmailNotificationType.NewBid,
                trip.RiderEmail,
                new NewBidEmail
                {
                    RiderName = riderUser.Name,
                    TripName = trip.Name,
                    DriverName = driverUser.Name,
                    DriverRating = driverProfile.Rating,
                    Price = (decimal)(request.CentsAmount / 100.0),
                    StartTime = trip.StartTime,
                    PickupAddress = trip.PickupAddress,
                    DestinationAddress = trip.DestinationAddress,
                    RiderEmail = trip.RiderEmail,
                }
            );

            return Ok();
        }

        /// <summary>
        /// Delete a bid for a trip.
        /// </summary>
        /// <param name="tripId">The ID of the trip to delete the user's bid from</param>
        /// <returns>
        /// <see cref="OkResult"/> if the bid was successfully deleted,
        /// <see cref="BadRequestObjectResult"/> if the tripId is invalid or the trip is either confirmed or in the past,
        /// <see cref="UnauthorizedResult"/> if the user is not a driver
        /// </returns>
        [HttpDelete]
        [Route("bid")]
        [Authorize]
        [SwaggerOperation(Summary = "Delete a bid for a trip.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The bid was successfully deleted.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The tripId is invalid or the trip is either confirmed or in the past.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a driver.")]
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

            if (!String.IsNullOrEmpty(trip.DriverEmail) || trip.StartTime.ToLocalTime() < DateTime.Now)
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

        private async Task<Dictionary<string, GeographicRoute>> GetTripIdToRouteMap(
            IEnumerable<Trip> trips,
            Dictionary<string, GeographicRoute>? cache
        )
        {
            if (cache != null)
                return cache;
            var tripRoutes = new Dictionary<string, GeographicRoute>();
            await Task.WhenAll(trips.Select(async trip =>
            {
                var route = await _geographicRouteService.GetGeographicRoute(
                    trip.PickupLong,
                    trip.PickupLat,
                    trip.DestinationLong,
                    trip.DestinationLat
                );
                tripRoutes.Add(trip.Id, route);
            }));
            return tripRoutes;
        }

        private async Task<Dictionary<string, List<Bid>>> GetTripIdToBidMap(
            IEnumerable<Trip> trips,
            Dictionary<string, List<Bid>>? cache
        )
        {
            if (cache != null)
                return cache;
            var tripBids = new Dictionary<string, List<Bid>>();
            await Task.WhenAll(trips.Select(async trip =>
            {
                var bids = await _bidService.GetTripBidsAsync(trip.Id);
                tripBids.Add(trip.Id, bids);
            }));
            return tripBids;
        }

        private async Task<Dictionary<string, uint?>> GetTripIdToMinBidAmountMap(
            IEnumerable<Trip> trips,
            Dictionary<string, uint?>? cache,
            Dictionary<string, List<Bid>>? bidCache
        )
        {
            if (cache != null)
                return cache;
            bidCache = await GetTripIdToBidMap(trips, bidCache);
            var tripMinBidAmounts = new Dictionary<string, uint?>();
            foreach (var trip in trips)
            {
                tripMinBidAmounts.Add(
                    trip.Id,
                    bidCache[trip.Id].Select(bid => bid.CentsAmount).DefaultIfEmpty().Min()
                );
            }
            return tripMinBidAmounts;
        }

        private async Task<Dictionary<string, DriverProfile>> GetBidIdToDriverMap(
            IEnumerable<Bid> bids,
            Dictionary<string, DriverProfile>? cache
        )
        {
            if (cache != null)
                return cache;
            var bidDrivers = new Dictionary<string, DriverProfile>();
            await Task.WhenAll(bids.Select(async bid =>
            {
                if (!bidDrivers.ContainsKey(bid.Id))
                {
                    var driver = await _driverProfileService.GetAsync(bid.DriverEmail);
                    bidDrivers.Add(bid.Id, driver);
                }
            }));
            return bidDrivers;
        }

        private async Task<Dictionary<string, UserCore>> GetBidIdToUserMap(
            IEnumerable<Bid> bids,
            Dictionary<string, UserCore>? cache
        )
        {
            if (cache != null)
                return cache;
            var bidUsers = new Dictionary<string, UserCore>();
            await Task.WhenAll(bids.Select(async bid =>
            {
                if (!bidUsers.ContainsKey(bid.Id))
                {
                    var user = await _userCoreService.GetAsync(bid.DriverEmail);
                    bidUsers.Add(bid.Id, user);
                }
            }));
            return bidUsers;
        }

        private async Task<List<SearchedTrip>> TripsToSearchedTrips(List<Trip> trips)
        {
            var tripRoutes = await GetTripIdToRouteMap(trips, null);
            var tripBids = await GetTripIdToBidMap(trips, null);
            var tripMinBids = await GetTripIdToMinBidAmountMap(trips, null, tripBids);
            var bidDrivers = await GetBidIdToDriverMap(tripBids.Values.SelectMany(x => x), null);
            var bidUsers = await GetBidIdToUserMap(tripBids.Values.SelectMany(x => x), null);

            var searchedTrips = trips.Select(trip => new SearchedTrip
            {
                TripId = trip.Id,
                TripName = trip.Name,
                Thumbnail = tripRoutes[trip.Id].Image,
                GeoJson = tripRoutes[trip.Id].GeoJson,
                StartTime = trip.StartTime,
                Distance = tripRoutes[trip.Id].Distance,
                Duration = tripRoutes[trip.Id].Duration,
                PickupAddress = trip.PickupAddress,
                DestinationAddress = trip.DestinationAddress,
                CurrentMinBidCentsAmount = tripMinBids[trip.Id],
                TripBids = tripBids[trip.Id].Select(bid => new SearchedBid
                {
                    DriverName = bidUsers[bid.Id].Name,
                    DriverRating = bidDrivers[bid.Id].Rating,
                    DriverNumRatings = bidDrivers[bid.Id].NumRatings,
                    CentsAmount = bid.CentsAmount
                }).ToList(),
            }).ToList();

            return searchedTrips;
        }

        /// <summary>
        /// Get all biddable trips.
        /// </summary>
        /// <returns>
        /// <see cref="OkObjectResult"/> if the biddable trips are successfully returned
        /// </returns>
        [HttpGet]
        [Route("allTrips")]
        [ProducesResponseType(typeof(List<SearchedTrip>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get all biddable trips.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully returned all biddable trips.")]
        public async Task<IActionResult> GetAllAvailableTrips()
        {
            var availableTrips = await GetEligibleMarketTrips();
            var searchedTrips = await TripsToSearchedTrips(availableTrips);
            return Ok(searchedTrips);
        }

        private double EuclideanDistance(double lat1, double long1, double lat2, double long2)
        {
            double dLat = lat2 - lat1;
            double dLong = long2 - long1;
            return Math.Sqrt(dLat * dLat + dLong * dLong);
        }

        private double TripEuclideanDistance(Trip trip)
        {
            return EuclideanDistance(
                trip.PickupLat,
                trip.PickupLong,
                trip.DestinationLat,
                trip.DestinationLong
            );
        }

        private double TripCurrentToStartDistance(Trip trip, TripMarketOptions options)
        {
            return EuclideanDistance(
                (double)options.CurrentLat,
                (double)options.CurrentLong,
                trip.PickupLat,
                trip.PickupLong
            );
        }

        private double TripEndToTargetDistance(Trip trip, TripMarketOptions options)
        {
            return EuclideanDistance(
                trip.DestinationLat,
                trip.DestinationLong,
                (double)options.TargetLat,
                (double)options.TargetLong
            );
        }

        private bool TripPassesMaxCurrentToStartDistance(Trip trip, TripMarketOptions options)
        {
            return TripCurrentToStartDistance(trip, options) <= options.MaxCurrentToStartDistance;
        }

        private bool TripPassesMaxEndToTargetDistance(Trip trip, TripMarketOptions options)
        {
            return TripEndToTargetDistance(trip, options) <= options.MaxEndToTargetDistance;
        }

        private async Task<List<Trip>> GetEligibleMarketTrips()
        {
            var allTrips = await _tripService.GetAllTripsAsync();
            return allTrips.Where(trip => String.IsNullOrEmpty(trip.DriverEmail) && trip.StartTime.ToLocalTime() > DateTime.Now).ToList();
        }

        private async Task<List<Trip>> GetFilteredAndSortedTrips(TripMarketOptions options)
        {
            var allTrips = await GetEligibleMarketTrips();
            Dictionary<string, GeographicRoute>? tripRoutes = null;
            Dictionary<string, uint?>? tripMinBidAmounts = null;

            IEnumerable<Trip> someTrips = allTrips;

            if (options.MaxCurrentToStartDistance != null)
            {
                if (options.CurrentLat == null || options.CurrentLong == null)
                    throw new ArgumentException("Must include current coordinates");
                someTrips = someTrips.Where(trip => TripPassesMaxCurrentToStartDistance(trip, options));
            }

            if (options.MaxEndToTargetDistance != null)
            {
                if (options.TargetLat == null || options.TargetLong == null)
                    throw new ArgumentException("Must include target coordinates");
                someTrips = someTrips.Where(trip => TripPassesMaxEndToTargetDistance(trip, options));
            }

            if (options.MaxEuclideanDistance != null)
            {
                someTrips = someTrips.Where(trip => TripEuclideanDistance(trip) <= options.MaxEuclideanDistance);
            }

            if (options.MaxRouteDistance != null)
            {
                tripRoutes = await GetTripIdToRouteMap(allTrips, tripRoutes);
                someTrips = someTrips.Where(trip => tripRoutes[trip.Id].Distance <= options.MaxRouteDistance);
            }

            if (options.MinCurrentMinBid != null)
            {
                tripMinBidAmounts = await GetTripIdToMinBidAmountMap(allTrips, tripMinBidAmounts, null);
                someTrips = someTrips.Where(trip => tripMinBidAmounts[trip.Id] >= options.MinCurrentMinBid);
            }

            var filteredTrips = someTrips.ToList();

            IOrderedEnumerable<Trip> sortedTrips = null!;
            foreach (var sortMethod in options.SortMethods)
            {
                switch (sortMethod)
                {
                    case "euclideanDistance":
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(TripEuclideanDistance);
                        else
                            sortedTrips = sortedTrips.ThenBy(TripEuclideanDistance);
                        break;
                    case "routeDistance":
                        tripRoutes = await GetTripIdToRouteMap(filteredTrips, tripRoutes);
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(trip => tripRoutes[trip.Id].Distance);
                        else
                            sortedTrips = sortedTrips.ThenBy(trip => tripRoutes[trip.Id].Distance);
                        break;
                    case "routeDuration":
                        tripRoutes = await GetTripIdToRouteMap(filteredTrips, tripRoutes);
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(trip => tripRoutes[trip.Id].Duration);
                        else
                            sortedTrips = sortedTrips.ThenBy(trip => tripRoutes[trip.Id].Duration);
                        break;
                    case "currentToStartDistance":
                        if (options.CurrentLat == null || options.CurrentLong == null)
                            throw new ArgumentException("Must include current coordinates");
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(trip => TripCurrentToStartDistance(trip, options));
                        else
                            sortedTrips = sortedTrips.ThenBy(trip => TripCurrentToStartDistance(trip, options));
                        break;
                    case "endToTargetDistance":
                        if (options.TargetLat == null || options.TargetLong == null)
                            throw new ArgumentException("Must include target coordinates");
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(trip => TripEndToTargetDistance(trip, options));
                        else
                            sortedTrips = sortedTrips.ThenBy(trip => TripEndToTargetDistance(trip, options));
                        break;
                    case "currentMinBid":
                        tripMinBidAmounts = await GetTripIdToMinBidAmountMap(filteredTrips, tripMinBidAmounts, null);
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(trip => tripMinBidAmounts[trip.Id]);
                        else
                            sortedTrips = sortedTrips.ThenBy(trip => tripMinBidAmounts[trip.Id]);
                        break;
                    case "startTime":
                        if (sortedTrips == null)
                            sortedTrips = filteredTrips.OrderBy(trip => trip.StartTime);
                        else
                            sortedTrips = sortedTrips.ThenBy(trip => trip.StartTime);
                        break;
                    default:
                        throw new ArgumentException("Invalid sort method");
                }
            }

            List<Trip> finalTrips;
            if (sortedTrips != null)
                finalTrips = sortedTrips.ToList();
            else
                finalTrips = filteredTrips;

            return finalTrips;
        }

        /// <summary>
        /// Get biddable trips and filter and sort using options.
        /// </summary>
        /// <param name="options">The filtering and searching options form.</param>
        /// <returns>
        /// <see cref="OkObjectResult"/> if the biddable trips are successfully returned,
        /// <see cref="BadRequestObjectResult"/> if the options are invalid
        /// </returns>
        [HttpGet]
        [HttpPost]
        [Route("tripMarket")]
        [ProducesResponseType(typeof(List<SearchedTrip>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get biddable trips and filter and sort using options.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully returned the biddable trips.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid options")]
        public async Task<IActionResult> GetAllAvailableTrips([FromBody] TripMarketOptions options)
        {
            List<Trip> trips;
            try
            {
                trips = await GetFilteredAndSortedTrips(options);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            var searchedTrips = await TripsToSearchedTrips(trips);
            return Ok(searchedTrips);
        }

        /// <summary>
        /// Rate a rider.
        /// </summary>
        /// <param name="giveRating">The rating form.</param>
        /// <returns>
        /// <see cref="OkResult"/> if the rider was successfully rated,
        /// <see cref="BadRequestObjectResult"/> if the user is invalid or not authenticated,
        /// <see cref="UnauthorizedResult"/> if the user is not a driver
        /// </returns>
        [Authorize]
        [HttpPost]
        [Route("rating")]
        [SwaggerOperation(Summary = "Rate a rider.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully rated the rider.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user/auth or trip.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not a driver.")]
        public async Task<IActionResult> RateRider([FromBody] GiveRating giveRating)
        {
            ClaimsPrincipal currentUser = this.User;

            if (currentUser == null) { return BadRequest(new { Error = "Invalid User/Auth" }); };

            var role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (role != "driver") { return Unauthorized(); }

            var trip = await _tripService.GetTripByIdAsync(giveRating.TripId);
            if (trip == null) { return BadRequest(new { Error = "Trip does not exist" }); }

            var driverEmail = currentUser.FindFirstValue(ClaimTypes.Name);
            if (driverEmail != trip.DriverEmail) { return Unauthorized(); }

            var riderEmail = trip.RiderEmail;
            if (string.IsNullOrEmpty(riderEmail)) { return BadRequest(new { Error = "Trip does not have a rider (somehow)" }); }

            if (trip.StartTime.ToLocalTime() >= DateTime.Now) { return BadRequest(new { Error = "Trip has not been taken yet" }); }

            if (trip.RiderHasBeenRated) { return BadRequest(new { Error = "Rider has already been rated for this trip" }); }

            var rider = await _riderProfileService.GetAsync(riderEmail);
            if (rider == null) { return BadRequest(new { Error = "Rider does not exist" }); }

            if (rider.Rating == null)
                rider.Rating = 0;
            if (rider.NumRatings == null)
                rider.NumRatings = 0;

            var totalRatings = rider.Rating * (double)rider.NumRatings;

            rider.NumRatings++;

            rider.Rating = (totalRatings + (double)giveRating.RatingGiven) / (double)rider.NumRatings;

            await _riderProfileService.UpdateAsync(rider.Email, rider);

            trip.RiderHasBeenRated = true;

            await _tripService.UpdateAsync(giveRating.TripId, trip);
            return Ok();
        }
    }
}
