using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        return Ok(new { Message = "Admin login successful" });
    }

    [HttpGet("pending-users")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var users = await _userService.GetPendingUsersAsync();
        return Ok(users);
    }

    [HttpPost("approve-user/{email}")]
    public async Task<IActionResult> ApproveUser(string email, [FromBody] UserRole role)
    {
        var success = await _userService.ApproveUserAsync(email, role);
        if (!success) return NotFound("User not found or not pending");
        return Ok("User approved successfully");
    }

    [HttpPost("reject-user/{email}")]
    public async Task<IActionResult> RejectUser(string email, [FromBody] string reason = "")
    {
        var success = await _userService.RejectUserAsync(email, reason);
        if (!success) return NotFound("User not found or not pending");
        return Ok("User rejected successfully");
    }
}
