using ClickHealthBackend.Data;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using MongoDB.Driver;

namespace ClickHealthBackend.Services.Implementations
{
    public class ContentService:IContentService
    {
        private readonly IMongoCollection<Content> _contentCollection;

        public ContentService(MongoDbContext context)
        {
            _contentCollection = (IMongoCollection<Content>?)context.Contents;
        }

        public async Task<string> UploadContentAsync(Content newContent)
        {
            newContent.UploadedAt = DateTime.UtcNow;
            newContent.Status = ContentStatus.PendingApproval;

            await _contentCollection.InsertOneAsync(newContent);
            return newContent.ContentId;
        }
    }
}
