using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetAdminByEmailAsync(string email);

    }
}