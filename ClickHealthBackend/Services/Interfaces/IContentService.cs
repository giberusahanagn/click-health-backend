using ClickHealthBackend.Models;

namespace ClickHealthBackend.Services.Interfaces
{
    public interface IContentService
    {
        Task<Content?> UploadContentAsync(Content newContent);
    }
}
