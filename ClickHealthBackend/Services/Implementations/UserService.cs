using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // -------------------------------
        // LOGIN USING PASSWORD
        // -------------------------------
        public async Task<User?> UserLoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || !user.IsApproved) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;

            // Force reset if temporary password
            if (user.MustResetPassword) return user;

            return user;
        }

        // -------------------------------
        // LOGIN USING OTP
        // -------------------------------
        public async Task<User?> UserLoginWithOtpAsync(string email, string otp)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || !user.IsApproved || string.IsNullOrEmpty(user.Totp)) return null;

            if (user.TotpGeneratedAt == null ||
                (DateTime.UtcNow - user.TotpGeneratedAt.Value).TotalMinutes > otpValidityMinutes)
                return null;

            if (user.Totp != otp) return null;

            user.Totp = null;
            user.TotpGeneratedAt = null;
            await _repo.UpdateAsync(user.UserId, user);

            return user;
        }

        // -------------------------------
        // SEND OTP
        // -------------------------------
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
                "ClickHealth OTP",
                $"Your OTP is <b>{otp}</b>. It expires in {otpValidityMinutes} minutes."
            );

            return true;
        }

        // -------------------------------
        // VERIFY OTP (LOGIN)
        // -------------------------------
        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Totp == null) return false;

            if (user.TotpGeneratedAt == null ||
                (DateTime.UtcNow - user.TotpGeneratedAt.Value).TotalMinutes > otpValidityMinutes)
                return false;

            if (user.Totp != otp) return false;

            user.Totp = null;
            user.TotpGeneratedAt = null;
            await _repo.UpdateAsync(user.UserId, user);

            return true;
        }

        // -------------------------------
        // VERIFY OTP FOR RESET
        // -------------------------------
        public async Task<bool> VerifyOtpForResetAsync(string email, string otp)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Totp == null) return false;

            if (user.TotpGeneratedAt == null ||
                (DateTime.UtcNow - user.TotpGeneratedAt.Value).TotalMinutes > otpValidityMinutes)
                return false;

            if (user.Totp != otp) return false;

            user.Totp = null;
            user.TotpGeneratedAt = null;
            await _repo.UpdateAsync(user.UserId, user);

            return true;
        }

        // -------------------------------
        // UPDATE PASSWORD
        // -------------------------------
        public async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.MustResetPassword = false;
            user.LastPasswordChangeAt = DateTime.UtcNow;

            await _repo.UpdateAsync(user.UserId, user);
            return true;
        }

        // -------------------------------
        // REGISTRATION
        // -------------------------------
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
                Status = UserStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateAsync(newUser);
            return newUser;
        }

        // -------------------------------
        // ADMIN APPROVE USER
        // -------------------------------
        public async Task<bool> ApproveUserAsync(string email, UserRole role)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Status != UserStatus.Pending) return false;

            var tempPassword = Guid.NewGuid().ToString().Substring(0, 8);
            user.Password = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            user.Role = role;
            user.IsApproved = true;
            user.MustResetPassword = true;
            user.Status = UserStatus.Approved;

            await _repo.UpdateAsync(user.UserId, user);

            await _emailService.SendEmailAsync(
                user.Email,
                "ClickHealth Account Approved",
                $"Welcome! Temporary password: <b>{tempPassword}</b>. Please reset after first login."
            );

            return true;
        }

        // -------------------------------
        // ADMIN REJECT USER
        // -------------------------------
        public async Task<bool> RejectUserAsync(string email, string reason = null)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null || user.Status != UserStatus.Pending) return false;

            user.Status = UserStatus.Rejected;
            await _repo.UpdateAsync(user.UserId, user);

            await _emailService.SendEmailAsync(
                user.Email,
                "ClickHealth Account Rejected",
                $"Your account registration has been rejected.{(string.IsNullOrEmpty(reason) ? "" : $" Reason: {reason}")}"
            );

            return true;
        }

        // -------------------------------
        // GET USERS BY STATUS
        // -------------------------------
        public async Task<List<User>> GetUsersByStatusAsync(UserStatus status) =>
            await _repo.GetUsersByStatusAsync(status);

        public async Task<List<User>> GetPendingUsersAsync() =>
            await _repo.GetUsersByStatusAsync(UserStatus.Pending);

        public async Task<User?> AdminLoginAsync(string email, string password)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) return null;

            // Ensure user is Admin
            if (user.Role != UserRole.Admin) return null;

            // Ensure account is approved and active
            if (!user.IsApproved || !user.IsActive) return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;

            return user;
        }

    }
}
