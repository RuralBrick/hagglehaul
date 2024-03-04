using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    /// <summary>
    /// Corresponds to Trip collection in the database.
    /// </summary>
    public interface ITripService
    {
        /// <summary>
        /// Gets all trips in the database. Useful for trip discovery.
        /// </summary>
        /// <returns></returns>
        Task<List<Trip>> GetAllTripsAsync();
        /// <summary>
        /// Gets all trips which the rider has created. Found through the riderEmail field of the trip.
        /// </summary>
        /// <param name="email">The valid email address of the rider.</param>
        /// <returns></returns>
        Task<List<Trip>> GetRiderTripsAsync(string email);
        /// <summary>
        /// Gets all trips which the driver has created. Found through the driverEmail field of the trip.
        /// </summary>
        /// <param name="email">The valid email address of the driver.</param>
        /// <returns></returns>
        Task<List<Trip>> GetDriverTripsAsync(string email);
        /// <summary>
        /// Gets a trip by its ID.
        /// </summary>
        /// <param name="id">The Trip ID of the desired trip.</param>
        /// <returns></returns>
        Task<Trip> GetTripByIdAsync(string id);
        /// <summary>
        /// Creates a new trip.
        /// </summary>
        /// <param name="trip">The <see cref="Trip"/> object to insert. Note that we do not
        /// check the object for correctness.</param>
        /// <returns></returns>
        Task<Trip> CreateAsync(Trip trip);
        /// <summary>
        /// Deletes a trip by its ID.
        /// </summary>
        /// <param name="id">The Trip ID to delete.</param>
        /// <returns></returns>
        Task DeleteAsync(string id);
        /// <summary>
        /// Updates a Trip object.
        /// </summary>
        /// <param name="id">The Trip ID to update.</param>
        /// <param name="tripIn">The <see cref="Trip"/> object to update. Note that we do not
        /// check the object for correctness.</param>
        /// <returns></returns>
        Task UpdateAsync(string id, Trip tripIn);
    }

    /// <summary>
    /// See <see cref="ITripService"/>.
    /// </summary>
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
        
        public async Task DeleteAsync(string id)
        {
            await _tripCollection.DeleteOneAsync(trip => trip.Id == id);
        }
        
        public async Task UpdateAsync(string id, Trip tripIn)
        {
            await _tripCollection.ReplaceOneAsync(trip => trip.Id == id, tripIn);
        }
    }
}
