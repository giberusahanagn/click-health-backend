using ClickHealthBackend.DTOs;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;

namespace ClickHealthBackend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IEmailService _emailService;
        private readonly int otpValidityMinutes = 3;

        public UserService(IUserRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        // --- Admin login (password only)
        public async Task<User?> AdminLoginAsync(string email, string password)
        {
            var admin = await _repo.GetByEmailAsync(email);
            if (admin == null || admin.Role != Enums.UserRole.Admin)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, admin.Password)) return null;

            // Generate OTP for admin
            await SendOtpAsync(email);
            return admin;
        }

        public async Task<User?> UserLoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || !user.IsApproved) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;

            // Generate OTP for user
            await SendOtpAsync(email);
            return user;
        }

        public async Task<User> RegisterUserAsync(RegistrationRequestDto request)
        {
            if (await _repo.ExistsAsync(request.Email))
                throw new Exception("Email already exists");

            var newUser = new User
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                Phone = request.Phone,
                Specialty = request.Specialty,
                Territory = request.Territory,
                PreferredLanguage = request.PreferredLanguage,
                IsApproved = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(newUser);
            return newUser;
        }

        public async Task<bool> ApproveUserAndSendCredentialsAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return false;

            // Generate password
            var password = Guid.NewGuid().ToString().Substring(0, 8);
            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            user.IsApproved = true;

            await _repo.UpdateAsync(user.UserId, user);

            // Send credentials via email
            await _emailService.SendEmailAsync(
                user.Email,
                "ClickHealth Account Approved",
                $"Your account has been approved.\nEmail: {user.Email}\nPassword: {password}"
            );

            return true;
        }

        // --- OTP generation ---
        public async Task<bool> SendOtpAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || !user.IsApproved) return false;

            var otp = new Random().Next(100000, 999999).ToString();
            user.Totp = otp;
            user.TotpGeneratedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user.UserId, user);

            await _emailService.SendEmailAsync(
                user.Email,
                "Your ClickHealth Login OTP",
                $"Your OTP is {otp}. It expires in {otpValidityMinutes} minutes."
            );

            return true;
        }

        public async Task<User?> VerifyOtpAsync(string email, string otp)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Totp == null) return null;

            if (user.TotpGeneratedAt == null ||
                (DateTime.UtcNow - user.TotpGeneratedAt.Value).TotalMinutes > otpValidityMinutes)
            {
                return null; // expired
            }

            if (user.Totp != otp) return null; // invalid

            // Clear OTP after verification
            user.Totp = null;
            user.TotpGeneratedAt = null;
            await _repo.UpdateAsync(user.UserId, user);

            return user;
        }

        public async Task<List<User>> GetPendingUsersAsync() =>
            await _repo.GetPendingUsersAsync();
    }
}
