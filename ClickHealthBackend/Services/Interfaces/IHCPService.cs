using ClickHealthBackend.Models;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IHCPService
    {
        Task<string> GeneratePatientInviteLinkAsync(string hcpUserId, string contentId, string patientEmail);
        Task<IEnumerable<Content>> GetApprovedContentAsync();
        Task<bool> LogQuizCompletionAsync(string hcpUserId, string contentId, Dictionary<string, object> quizResponses);
    }
}
