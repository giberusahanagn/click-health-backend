using ClickHealthBackend.Data;
using ClickHealthBackend.DTOs;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Services.Interfaces;
using MongoDB.Driver;

namespace ClickHealthBackend.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly IMongoCollection<Content> _contentCollection;

        public ContentService(MongoDbContext context)
        {
            _contentCollection = (IMongoCollection<Content>?)context.Contents;
        }

        public async Task<string> UploadContentAsync(ContentDTO newContentDto, string uploadedByUserId, string uploadedByUserName)
        {
            if (newContentDto == null)
                throw new ArgumentNullException(nameof(newContentDto));

            var content = new Content
            {
                ContentId = Guid.NewGuid().ToString(),
                Therapy = newContentDto.MedicalName,
                Language = newContentDto.ContentLanguage,
                Description = newContentDto.ContentDescription,
                FileUrl = newContentDto.PdfUrl,
                ThumbnailUrl = newContentDto.VideoUrl,
                ReviewDate = newContentDto.StartDate,
                ExpiryDate = newContentDto.ExpiresOn,
                UploadedAt = DateTime.UtcNow,
                Status = ContentStatus.PendingApproval,

                // ✅ Temporary uploader values
                UploadedByUserId = uploadedByUserId,
                UploadedByUserName = uploadedByUserName
            };

            await _contentCollection.InsertOneAsync(content);

            return content.ContentId;
        }

        public async Task<List<Content>> GetAllContentsAsync()
        {
            return await _contentCollection.Find(_ => true).ToListAsync();
        }

        // ✅ Filter contents by status
        public async Task<List<MedicalContentResponseDto>> GetContentsByStatusAsync(ContentStatus status)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.Status, status);
            var contents = await _contentCollection.Find(filter).ToListAsync();

            var result = contents.Select(c => new MedicalContentResponseDto
            {
                MedicalName = c.Therapy,
                ContentLanguage = c.Language,
                Description = c.Description,
                ViewPdf = c.FileUrl,
                ViewVideo = c.ThumbnailUrl,
                EndDate = c.ExpiryDate,
                Actions = "View/Edit/Delete" // just for frontend buttons or icons
            }).ToList();

            return result;
        }

        // ✅ Update content status
        public async Task<bool> UpdateContentStatusAsync(string contentId, ContentStatus newStatus, string approverName, string notes)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.ContentId, contentId);
            var update = Builders<Content>.Update
                .Set(c => c.Status, newStatus)
                .Set(c => c.ApprovedByUserName, approverName)
                .Set(c => c.ApproverNotes, notes);

            var result = await _contentCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<List<ContentWorkflowDTO>> GetContentWorkflowByUploaderAsync(string uploaderId)
        {
            var filter = Builders<Content>.Filter.Eq(c => c.UploadedByUserId, uploaderId);
            var contents = await _contentCollection.Find(filter).ToListAsync();

            var result = contents.Select(c => new ContentWorkflowDTO
            {
                ContentId = c.ContentId,
                Content = c.Description,          // or c.FileName if you have it
                ContentCreator = c.UploadedByUserName,
                UploadedAt = c.UploadedAt,
                Status = c.Status.ToString(),
                Approver = c.ApprovedByUserName,       // this should be set when PM approves
                Notes = c.ApproverNotes
            }).ToList();

            return result;
        }

        public async Task<List<ContentWorkflowDTO>> GetContentWorkflowAsync()
        {
            var contents = await _contentCollection.Find(_ => true).ToListAsync();

            var workflowList = contents.Select(c => new ContentWorkflowDTO
            {
                ContentId = c.ContentId,
                Content = c.Description ?? c.Therapy,
                ContentCreator = c.UploadedByUserName,
                UploadedAt = c.UploadedAt,
                Status = c.Status.ToString(),
                Approver = c.ApprovedByUserName,
                Notes = c.ApproverNotes
            }).ToList();

            return workflowList;
        }


        //Task<List<Content>> IContentService.GetContentsByStatusAsync(ContentStatus status)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
