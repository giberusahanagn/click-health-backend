using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using ClickHealthBackend.Enums;
using MongoDB.Bson;

namespace ClickHealthBackend.Services.Implementation
{
    public class HCPService : IHCPService
    {
        private readonly IContentRepository _contentRepository;
        private readonly IPatientInviteRepository _patientInviteRepository;
        private readonly IHCPActivityRepository _hcpActivityRepository;
        private readonly IEmailService _emailService;

        public HCPService(
            IContentRepository contentRepository,
            IPatientInviteRepository patientInviteRepository,
            IHCPActivityRepository hcpActivityRepository,
            IEmailService emailService)
        {
            _contentRepository = contentRepository;
            _patientInviteRepository = patientInviteRepository;
            _hcpActivityRepository = hcpActivityRepository;
            _emailService = emailService;
        }

        public async Task<string> GeneratePatientInviteLinkAsync(string hcpUserId, string contentId, string patientEmail)
        {
            // 1. Verification: Ensure content is approved
            var content = await _contentRepository.GetByIdAsync(contentId);
            if (content == null || content.Status != ContentStatus.Approved)
            {
                return null; // Cannot generate a link for unapproved content
            }

            // 2. Create Invite Record
            var inviteCode = Guid.NewGuid().ToString("N");
            var invite = new PatientInvite
            {
                InviteCode = inviteCode,
                HcpUserId = hcpUserId,
                ContentId = contentId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsActive = true,
                MaxUses = 1
            };
            await _patientInviteRepository.CreateAsync(invite);

            // 3. Send Email using the system's email service
            string subject = "An Educational Resource from Your Healthcare Provider";
            string body = $"Hello,\n\nYour doctor has shared an educational resource with you. Please click the link below to view the content:\n\nhttps://your-app-url.com/patient-portal?inviteCode={inviteCode}\n\nThis link is private and will expire in 7 days.\n\nThank you,\nClickHealth Team";
            await _emailService.SendEmailAsync(patientEmail, subject, body);

            // 4. Log Activity
            await _hcpActivityRepository.CreateAsync(new HCPActivity
            {
                HcpUserId = hcpUserId,
                HcpActivityType = HCPActivityType.Share,
                ContentId = contentId,
                Timestamp = DateTime.UtcNow,
            });

            return inviteCode;
        }

        public async Task<IEnumerable<Content>> GetApprovedContentAsync()
        {
            // Retrieve all content and filter it in the service layer to show only approved items
            var allContent = await _contentRepository.GetAllAsync();
            return allContent.Where(c => c.Status == ContentStatus.Approved).ToList();
        }

        public async Task<bool> LogQuizCompletionAsync(string hcpUserId, string contentId, Dictionary<string, object> quizResponses)
        {
            // Log the quiz completion and responses for the HCP
            await _hcpActivityRepository.CreateAsync(new HCPActivity
            {
                HcpUserId = hcpUserId,
                ContentId = contentId,
                HcpActivityType = HCPActivityType.Quiz,
                Timestamp = DateTime.UtcNow,
                // Store the quiz responses flexibly as BsonDocument metadata
                Metadata = new BsonDocument(quizResponses)
            });
            return true;
        }
    }
}