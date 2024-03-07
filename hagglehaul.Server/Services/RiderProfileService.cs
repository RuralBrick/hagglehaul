using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    /// <summary>
    /// Corresponds to RiderProfile collection in the database.
    /// </summary>
    public interface IRiderProfileService
    {
        /// <summary>
        /// Gets a rider profile by the user's email
        /// </summary>
        /// <param name="email">The valid email address of an existing rider</param>
        /// <returns></returns>
        Task<RiderProfile> GetAsync(string email);
        /// <summary>
        /// Creates a new rider profile
        /// </summary>
        /// <param name="riderProfile">The <see cref="RiderProfile"/> object. Note that
        /// we do not check for correctness of the object.</param>
        /// <returns></returns>
        Task<RiderProfile> CreateAsync(RiderProfile riderProfile);
        /// <summary>
        /// Updates a rider profile
        /// </summary>
        /// <param name="email">The valid email address of an existing rider</param>
        /// <param name="riderProfileIn">The <see cref="RiderProfile"/> object. Note that
        /// we do not check for the correctness of the object.</param>
        /// <returns></returns>
        Task UpdateAsync(string email, RiderProfile riderProfileIn);
    }

    /// <summary>
    /// See <see cref="IRiderProfileService"/>.
    /// </summary>
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
