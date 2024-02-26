using System.Security.Claims;
using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<Dictionary<string, GeographicRoute>> GetTripIdToRouteMap(
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

        [HttpGet]
        [Route("allTrips")]
        [ProducesResponseType(typeof(List<SearchedTrip>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAvailableTrips()
        {
            var allTrips = await _tripService.GetAllTripsAsync();
            var availableTrips = allTrips.Where(trip => trip.DriverEmail == null).ToList();
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

        public async Task<List<Trip>> GetFilteredAndSortedTrips(TripMarketOptions options)
        {
            var allTrips = await _tripService.GetAllTripsAsync();
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
                        if (options.TargetLat == null || options.TargetLong == null)
                            throw new ArgumentException("Must include target coordinates");
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


        [HttpGet]
        [Route("tripMarket")]
        [ProducesResponseType(typeof(List<SearchedTrip>), StatusCodes.Status200OK)]
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
            var searchedTrips = TripsToSearchedTrips(trips);
            return Ok(searchedTrips);
        }
    }
}
