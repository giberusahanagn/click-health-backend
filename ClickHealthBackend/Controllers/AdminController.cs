using ClickHealthBackend.DTOs;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var admin = await _userService.AdminLoginAsync(dto.Email, dto.Password);
        if (admin == null) return Unauthorized("Invalid admin credentials");

        return Ok(new { Message = "Password verified. OTP sent to admin email." });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
    {
        var admin = await _userService.VerifyOtpAsync(dto.Email, dto.Otp);
        if (admin == null) return Unauthorized("Invalid or expired OTP");

        return Ok(new { Message = "Admin login successful", Email = admin.Email, Role = admin.Role });
    }

    [HttpGet("pending-users")]
    public async Task<IActionResult> GetPendingUsers() =>
        Ok(await _userService.GetPendingUsersAsync());

    [HttpPost("approve-user/{email}")]
    public async Task<IActionResult> ApproveUser(string email)
    {
        var success = await _userService.ApproveUserAndSendCredentialsAsync(email);
        if (!success) return NotFound("User not found");
        return Ok("User approved and credentials sent via email");
    }
}
