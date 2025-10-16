using ClickHealthBackend.Models;
using ClickHealthBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> AdminLoginAsync(string email, string password);
        Task<User?> UserLoginAsync(string email, string password);
        Task<User> RegisterUserAsync(RegistrationRequestDto request);
        Task<List<User>> GetPendingUsersAsync();
        Task<bool> ApproveUserAndSendCredentialsAsync(string email);

        // OTP methods (users + admins)
        Task<bool> SendOtpAsync(string email);
        Task<User?> VerifyOtpAsync(string email, string otp);
    }
}
