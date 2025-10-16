using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Implementations;
using Microsoft.AspNetCore.Mvc;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ContentService _contentService;

        public ContentController(ContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadContent([FromBody] Content content)
        {
            if (content == null || string.IsNullOrEmpty(content.Title))
                return BadRequest("Invalid content payload.");

            var contentId = await _contentService.UploadContentAsync(content);
            return Ok(new { contentId, message = "Content uploaded successfully." });
        }
    }
}
