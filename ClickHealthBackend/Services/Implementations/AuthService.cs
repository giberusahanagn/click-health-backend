using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;

namespace ClickHealthBackend.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AdminLoginAsync(string email, string password)
        {
            var admin = await _userRepository.GetAdminByEmailAsync(email);

            if (admin == null) return null;

            // For demo purposes: hardcoded password, replace with proper hashing
            if (password != "admin123") return null;

            return admin;
        }
    }
}
