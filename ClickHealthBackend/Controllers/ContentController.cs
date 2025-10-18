using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClickHealthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadContent([FromBody] ContentDTO contentDto)
        {
            if (contentDto == null || string.IsNullOrEmpty(contentDto.MedicalName))
                return BadRequest("Invalid request: 'MedicalName' field is required.");

            try
            {
                // ✅ Temporary default user name until JWT integration
                string defaultUploaderName = "TeamUser";
                string defaultUploaderId = "TEMP123";

                var contentId = await _contentService.UploadContentAsync(contentDto, defaultUploaderId, defaultUploaderName);

                return Ok(new
                {
                    ContentId = contentId,
                    Message = $"Content uploaded successfully by {defaultUploaderName}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // ✅ 1. Get all approved contents
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedContents()
        {
            var contents = await _contentService.GetContentsByStatusAsync(ContentStatus.Approved);
            return contents.Any() ? Ok(contents) : NotFound("No approved contents found.");
        }

        // ✅ 2. Get all rejected contents
        [HttpGet("rejected")]
        public async Task<IActionResult> GetRejectedContents()
        {
            var contents = await _contentService.GetContentsByStatusAsync(ContentStatus.Rejected);
            return contents.Any() ? Ok(contents) : NotFound("No rejected contents found.");
        }

        // ✅ 3. Get all pending contents
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingContents()
        {
            var contents = await _contentService.GetContentsByStatusAsync(ContentStatus.PendingApproval);
            return contents.Any() ? Ok(contents) : NotFound("No pending contents found.");
        }


        // ✅ Update content status (Approve / Reject / Pending)
        [HttpPut("update-status/{contentId}")]
        public async Task<IActionResult> UpdateContentStatus(
      string contentId,
      [FromQuery] ContentStatus newStatus,
      [FromQuery] string? notes)
        {
            if (string.IsNullOrEmpty(contentId))
                return BadRequest("Content ID is required.");

            // ✅ Temporary approver name (later, get from JWT)
            string approverName = "ProductManager01";

            var updated = await _contentService.UpdateContentStatusAsync(contentId, newStatus, approverName, notes);

            if (!updated)
                return NotFound($"No content found with ID: {contentId}");

            return Ok(new
            {
                Message = $"Content status updated to '{newStatus}' by {approverName}.",
                ContentId = contentId
            });
        }


   

        [HttpGet("workflow")]
        public async Task<IActionResult> GetContentWorkflow()
        {
            var result = await _contentService.GetContentWorkflowAsync();
            return Ok(result);
        }


    }
}
