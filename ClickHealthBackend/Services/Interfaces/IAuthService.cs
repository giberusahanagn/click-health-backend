using ClickHealthBackend.Models;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> AdminLoginAsync(string email, string password);
    }
}
