using ClickHealthBackend.DTOs;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequestDto dto)
    {
        var user = await _userService.RegisterUserAsync(dto);
        return Ok(new { Message = "Registration successful. Wait for admin approval." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _userService.UserLoginAsync(dto.Email, dto.Password);
        if (user == null) return Unauthorized("Invalid credentials or not approved");

        return Ok(new { Message = "Password verified. OTP sent to email." });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
    {
        var user = await _userService.VerifyOtpAsync(dto.Email, dto.Otp);
        if (user == null) return Unauthorized("Invalid or expired OTP");

        return Ok(new { Message = "Login successful", Email = user.Email, Role = user.Role });
    }
}
