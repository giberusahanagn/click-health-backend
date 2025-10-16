using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ClickHealthDb");
            _usersCollection = database.GetCollection<User>("Users");
        }

        public async Task<User?> GetByIdAsync(string id) =>
            await _usersCollection.Find(u => u.UserId == id).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email) =>
            await _usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<List<User>> GetPendingUsersAsync() =>
            await _usersCollection.Find(u => u.IsApproved == false).ToListAsync();

        public async Task CreateAsync(User user) =>
            await _usersCollection.InsertOneAsync(user);

        public async Task<bool> UpdateAsync(string id, User user)
        {
            var result = await _usersCollection.ReplaceOneAsync(u => u.UserId == id, user);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> ExistsAsync(string email) =>
            await _usersCollection.Find(u => u.Email == email).AnyAsync();

        // Optional: Approve user by updating password and isApproved
        public async Task<bool> ApproveUserAsync(string email, string hashedPassword)
        {
            var update = Builders<User>.Update
                .Set(u => u.Password, hashedPassword)
                .Set(u => u.IsApproved, true);

            var result = await _usersCollection.UpdateOneAsync(u => u.Email == email, update);
            return result.ModifiedCount > 0;
        }
    }
}
