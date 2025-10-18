using ClickHealthBackend.Data;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using MongoDB.Driver;
using ClickHealthBackend.Enums;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class ContentRepository : IContentRepository
    {
        // Holds the strongly-typed collection reference
        private readonly IMongoCollection<Content> _content;

        public ContentRepository(MongoDbContext context)
        {
            // Initializes the collection using the strongly-typed property from MongoDbContext
            _content = context.Contents;
        }

        public async Task<Content> GetByIdAsync(string id) =>
            await _content.Find(c => c.ContentId == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Content>> GetPendingContentAsync() =>
            // Used by the Medical Chief's review queue
            await _content.Find(c => c.Status == ContentStatus.Pending).ToListAsync();

        public async Task<IEnumerable<Content>> GetAllAsync() =>
            // Used for dashboard and HCP content browsing (before service-layer filtering)
            await _content.Find(_ => true).ToListAsync();

        public async Task CreateAsync(Content content) =>
            await _content.InsertOneAsync(content);

        public async Task<bool> UpdateAsync(Content content)
        {
            // Used primarily by the Medical Service to update the status (Approved/Rejected)
            var result = await _content.ReplaceOneAsync(c => c.ContentId == content.ContentId, content);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
