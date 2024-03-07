using hagglehaul.Server.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MongoDB.Driver;
using System.Security.Cryptography;

namespace hagglehaul.Server.Services
{
    /// <summary>
    /// Corresponds to UserCore collection in the database, along with some utilities.
    /// </summary>
    public interface IUserCoreService
    {
        /// <summary>
        /// Gets a list of all UserCore objects in the database.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="UserCore"/> objects.</returns>
        Task<List<UserCore>> GetAsync();
        /// <summary>
        /// Gets a UserCore object by the user's email.
        /// </summary>
        /// <param name="email">The valid email of the user. Each email should correspond
        /// with only one UserCore object.</param>
        /// <returns></returns>
        Task<UserCore> GetAsync(string email);
        /// <summary>
        /// Creates a UserCore object.
        /// </summary>
        /// <param name="userCore">The <see cref="UserCore"/> object to insert. We do not
        /// check the object for correctness.</param>
        /// <returns></returns>
        Task<UserCore> CreateAsync(UserCore userCore);
        /// <summary>
        /// Updates a UserCore object.
        /// </summary>
        /// <param name="email">The valid email of the user.</param>
        /// <param name="userCoreIn">The <see cref="UserCore"/> object to insert. We do not
        /// check the object for correctness.</param>
        /// <returns></returns>
        Task UpdateAsync(string email, UserCore userCoreIn);
        /// <summary>
        /// Removes a UserCore object.
        /// </summary>
        /// <param name="email">The valid email of the user.</param>
        /// <returns></returns>
        Task RemoveAsync(string email);
        /// <summary>
        /// Creates a random salt and a hash from a password. Should use an algorithm similar to
        /// or stronger than <see cref="KeyDerivation.Pbkdf2"/>.
        /// </summary>
        /// <param name="password">The non-encoded password.</param>
        /// <param name="hash">The hash, encoded as a base 64 string.</param>
        /// <param name="salt">The salt, encoded as a base 64 string.</param>
        void CreatePasswordHash(string password, out string hash, out string salt);
        /// <summary>
        /// Verifies a password against a hash and a salt. Should use an algorithm similar to
        /// or stronger than <see cref="KeyDerivation.Pbkdf2"/>.
        /// </summary>
        /// <param name="password">The non-encoded password.</param>
        /// <param name="hash">The hash, encoded as a base 64 string.</param>
        /// <param name="salt">The salt, encoded as a base 64 string.</param>
        /// <returns><c>true</c> if the password matches the hash and salt, or
        /// <c>false</c> otherwise.</returns>
        bool ComparePasswordToHash(string password, string hash, string salt);
    }
    
    /// <summary>
    /// See <see cref="IUserCoreService"/>.
    /// </summary>
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
