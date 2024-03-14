using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    /// <summary>
    /// Corresponds to Bid collection in the database
    /// </summary>
    public interface IBidService
    {
        /// <summary>
        /// Get all bids for a driver
        /// </summary>
        /// <param name="email">The email (username) of the driver</param>
        /// <returns>A <see cref="List{T}" /> of <see cref="Bid"/> objects</returns>
        Task<List<Bid>> GetDriverBidsAsync(string email);
        /// <summary>
        /// Get all bids for a trip
        /// </summary>
        /// <param name="tripId">The Trip ID of the trip</param>
        /// <returns>A <see cref="List{T}" /> of <see cref="Bid"/> objects</returns>
        Task<List<Bid>> GetTripBidsAsync(string tripId);
        /// <summary>
        /// Create a new bid on a trip
        /// </summary>
        /// <param name="bid">A Bid object to be inserted. Note that we do
        /// not check for correctness of the inserted object.</param>
        /// <returns></returns>
        Task<Bid> CreateAsync(Bid bid);
        /// <summary>
        /// Update a bid
        /// </summary>
        /// <param name="id">The Bid ID of the bid</param>
        /// <param name="bidIn">The new Bid object. Note that we do not
        /// check for correctness of the inserted object.</param>
        /// <returns></returns>
        Task UpdateAsync(string id, Bid bidIn);
        /// <summary>
        /// Delete a bid
        /// </summary>
        /// <param name="id">The Bid ID of the bid</param>
        /// <returns></returns>
        Task DeleteAsync(string id);
        /// <summary>
        /// Delete all bids for a given trip. Useful when a trip is deleted.
        /// </summary>
        /// <param name="tripId">The Trip ID for which all bids will be deleted.</param>
        /// <returns></returns>
        Task DeleteByTripIdAsync(string tripId);
    }

    /// <summary>
    /// See <see cref="IBidService"/>.
    /// </summary>
    public class BidService : IBidService
    {
        public readonly IMongoCollection<Bid> _bidCollection;

        public BidService(IMongoDatabase database)
        {
            _bidCollection = database.GetCollection<Bid>("Bid");
        }

        public async Task<List<Bid>> GetDriverBidsAsync(string email)
        {
            var driverBids = await _bidCollection.Find(bid => bid.DriverEmail == email).ToListAsync();
            return driverBids;
        }

        public async Task<List<Bid>> GetTripBidsAsync(string tripId)
        {
            var tripBids = await _bidCollection.Find(bid => bid.TripId == tripId).ToListAsync();
            return tripBids;
        }

        public async Task<Bid> CreateAsync(Bid bid)
        {
            await _bidCollection.InsertOneAsync(bid);
            return bid;
        }
        
        public async Task UpdateAsync(string id, Bid bidIn) =>
            await _bidCollection.ReplaceOneAsync(bid => bid.Id == id, bidIn);
        
        public async Task DeleteAsync(string id) =>
            await _bidCollection.DeleteOneAsync(bid => bid.Id == id);
        
        public async Task DeleteByTripIdAsync(string tripId) =>
            await _bidCollection.DeleteManyAsync(bid => bid.TripId == tripId);
    }
}
