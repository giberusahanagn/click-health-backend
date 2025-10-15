using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
        {
            _users = context.Users;
        }

        public async Task<User?> GetAdminByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email && u.Role == Enums.UserRole.Admin).FirstOrDefaultAsync();
        }
    }
}
