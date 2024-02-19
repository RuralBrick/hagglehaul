using hagglehaul.Server.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MongoDB.Driver;
using System.Security.Cryptography;

namespace hagglehaul.Server.Services
{
    public interface IUserCoreService
    {
        Task<List<UserCore>> GetAsync();
        Task<UserCore> GetAsync(string email);
        Task<UserCore> CreateAsync(UserCore userCore);
        Task UpdateAsync(string email, UserCore userCoreIn);
        Task RemoveAsync(string email);
        void CreatePasswordHash(string password, out string hash, out string salt);
        bool ComparePasswordToHash(string password, string hash, string salt);
    }
    
    public class UserCoreService : IUserCoreService
    {
        private readonly IMongoCollection<UserCore> _userCoreCollection;
        public void CreatePasswordHash(string password, out string hash, out string salt)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(128 / 8);
            salt = Convert.ToBase64String(saltBytes);
            hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }

        public bool ComparePasswordToHash(string password, string hash, string salt)
        {
            string newHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return newHash == hash;
        }

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
