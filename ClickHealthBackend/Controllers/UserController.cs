using ClickHealthBackend.DTOs;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequestDto request)
        {
            var admin = await _authService.AdminLoginAsync(request.Email, request.Password);

            if (admin == null)
                return Unauthorized("Invalid admin credentials");

            // Optional: Generate JWT token (create JwtTokenHelper)
            var token = "dummy-token"; // replace with real JWT generation

            var response = new LoginResponseDto
            {
                Email = admin.Email,
                Role = admin.Role,
                Token = token
            };

            return Ok(response);
        }
    }
}
