using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    public class UserCoreService
    {
        private readonly IMongoCollection<UserCore> _userCoreCollection;

        public UserCoreService(IMongoDatabase database)
        {
            _userCoreCollection = database.GetCollection<UserCore>("UserCore");
        }

        public async Task<List<UserCore>> GetAsync() =>
            await _userCoreCollection.Find(_ => true).ToListAsync();

        public async Task<UserCore> GetAsync(string email) =>
            await _userCoreCollection.Find(userCore => userCore.Email == email).FirstOrDefaultAsync();

        public async Task<UserCore> CreateAsync(UserCore userCore)
        {
            await _userCoreCollection.InsertOneAsync(userCore);
            return userCore;
        }
        
        public async Task UpdateAsync(string email, UserCore userCoreIn) =>
            await _userCoreCollection.ReplaceOneAsync(userCore => userCore.Email == email, userCoreIn);

        public async Task RemoveAsync(string email) =>
            await _userCoreCollection.DeleteOneAsync(userCore => userCore.Email == email);
    }
}
