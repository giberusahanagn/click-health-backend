using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IContentService
    {
        //Task<string> UploadContentAsync(ContentDTO contentDto, string uploadedByUserId, string uploadedByUserName);

        //Task<List<Content>> GetAllContentsAsync();
        //Task<List<MedicalContentResponseDto>> GetContentsByStatusAsync(ContentStatus status);

        Task<string> UploadContentAsync(ContentDTO newContentDto, string uploadedByUserId, string uploadedByUserName);
        Task<List<Content>> GetAllContentsAsync();

        // ✅ Updated to return DTOs (not full Content)
        Task<List<MedicalContentResponseDto>> GetContentsByStatusAsync(ContentStatus status);

        // ✅ Update content status
        //Task<bool> UpdateContentStatusAsync(string contentId, ContentStatus newStatus);
        Task<bool> UpdateContentStatusAsync(string contentId, ContentStatus newStatus, string approverName, string notes);


        Task<List<ContentWorkflowDTO>> GetContentWorkflowAsync();


    }
}
