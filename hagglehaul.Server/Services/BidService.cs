﻿using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    public interface IBidService
    {
        Task<List<Bid>> GetDriverBidsAsync(string email);
        Task<List<Bid>> GetTripBidsAsync(string tripId);
        Task<Bid> CreateAsync(Bid bid);
    }

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
    }
}