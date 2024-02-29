using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    public interface IRiderProfileService
    {
        Task<RiderProfile> GetAsync(string email);
        Task<RiderProfile> CreateAsync(RiderProfile riderProfile);
        Task UpdateAsync(string email, RiderProfile riderProfileIn);
    }

    public class RiderProfileService : IRiderProfileService
    {
        private readonly IMongoCollection<RiderProfile> _riderProfileCollection;

        public RiderProfileService(IMongoDatabase database)
        {
            _riderProfileCollection = database.GetCollection<RiderProfile>("RiderProfile");
        }

        public async Task<RiderProfile> GetAsync(string email)
        {
            var riderProfile = await _riderProfileCollection.Find(riderProfile => riderProfile.Email == email).FirstOrDefaultAsync();
            return riderProfile;
        }

        public async Task<RiderProfile> CreateAsync(RiderProfile riderProfile)
        {
            await _riderProfileCollection.InsertOneAsync(riderProfile);
            return riderProfile;
        }

        public async Task UpdateAsync(string email, RiderProfile riderProfileIn) =>
            await _riderProfileCollection.ReplaceOneAsync(riderProfile => riderProfile.Email == email, riderProfileIn);
    }
}
