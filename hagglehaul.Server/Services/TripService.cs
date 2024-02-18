using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    public interface ITripService
    {
        Task<List<Trip>> GetAllTripsAsync();
        Task<List<Trip>> GetRiderTripsAsync(string email);
        Task<List<Trip>> GetDriverTripsAsync(string email);
        Task<Trip> GetTripByIdAsync(string id);
        Task<Trip> CreateAsync(Trip trip);
    }

    public class TripService : ITripService
    {
        public readonly IMongoCollection<Trip> _tripCollection;

        public TripService(IMongoDatabase database)
        {
            _tripCollection = database.GetCollection<Trip>("Trip");
        }

        public async Task<List<Trip>> GetAllTripsAsync()
        {
            var allTrips = await _tripCollection.Find(_ => true).ToListAsync();
            return allTrips;
        }

        public async Task<List<Trip>> GetRiderTripsAsync(string email)
        {
            var riderTrips = await _tripCollection.Find(trip => trip.RiderEmail == email).ToListAsync();
            return riderTrips;
        }

        public async Task<List<Trip>> GetDriverTripsAsync(string email)
        {
            var riderTrips = await _tripCollection.Find(trip => trip.DriverEmail == email).ToListAsync();
            return riderTrips;
        }
        
        public async Task<Trip> GetTripByIdAsync(string id)
        {
            var trip = await _tripCollection.Find(trip => trip.Id == id).FirstOrDefaultAsync();
            return trip;
        }

        public async Task<Trip> CreateAsync(Trip trip)
        {
            await _tripCollection.InsertOneAsync(trip);
            return trip;
        }
    }
}
