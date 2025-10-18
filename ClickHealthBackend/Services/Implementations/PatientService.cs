using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using ClickHealthBackend.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;
using System.Linq; // Required for LINQ methods

namespace ClickHealthBackend.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IPatientInviteRepository _patientInviteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IConsentRecordRepository _consentRecordRepository;
        private readonly IPatientEngagementRepository _patientEngagementRepository;

        public PatientService(
            IPatientInviteRepository patientInviteRepository,
            IContentRepository contentRepository,
            IConsentRecordRepository consentRecordRepository,
            IPatientEngagementRepository patientEngagementRepository)
        {
            _patientInviteRepository = patientInviteRepository;
            _contentRepository = contentRepository;
            _consentRecordRepository = consentRecordRepository;
            _patientEngagementRepository = patientEngagementRepository;
        }

        public async Task<Content> GetContentByInviteCodeAsync(string inviteCode)
        {
            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);

            // Check for invalid, expired, or deactivated invite
            if (invite == null || !invite.IsActive || invite.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            // NOTE: In a complete app, you would also check invite.UsedCount against invite.MaxUses 
            // and update the UsedCount before returning content.

            return await _contentRepository.GetByIdAsync(invite.ContentId);
        }

        public async Task<bool> RecordPatientConsentAsync(string inviteCode, string userIpAddress)
        {
            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);
            if (invite == null) return false;

            // Create a record of explicit consent for DPDP compliance
            var consentRecord = new ConsentRecord
            {
                // We use the HCP's ID for linkage, maintaining patient pseudonymity
                UserId = invite.HcpUserId,
                UserType = "Patient",
                ConsentType = "DPDP",
                IsGranted = true,
                GrantedAt = DateTime.UtcNow,
                IpAddress = userIpAddress
            };
            await _consentRecordRepository.CreateAsync(consentRecord);
            return true;
        }

        public async Task LogContentEngagementAsync(string inviteCode, string engagementType, int durationSeconds, string city, string language)
        {
            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);
            if (invite == null) return;

            // Attempt to parse the string engagementType into the Enum
            if (!Enum.TryParse(engagementType, true, out EngagementType parsedType))
            {
                // Optionally log an error, but prevent the record from being created with an invalid type
                return;
            }

            var engagement = new PatientEngagement
            {
                InviteCode = inviteCode,
                ContentId = invite.ContentId,
                CampaignId = invite.CampaignId,
                ViewedAt = DateTime.UtcNow, // Record the time the content was first viewed/engaged
                ConsentGiven = true, // Assumed since this log occurs after consent
                DurationSeconds = durationSeconds,
                City = city,
                Language = language,
                EngagementType = parsedType
            };
            await _patientEngagementRepository.CreateAsync(engagement);
        }

        public async Task<bool> LogQuizCompletionAsync(string inviteCode, string contentId, Dictionary<string, object> quizResponses)
        {
            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);
            if (invite == null) return false;

            var engagement = new PatientEngagement
            {
                InviteCode = inviteCode,
                ContentId = contentId,
                CampaignId = invite.CampaignId,
                CompletedAt = DateTime.UtcNow, // Record completion time
                ConsentGiven = true,
                QuizResponse = new BsonDocument(quizResponses),
                // Set the specific engagement type for the quiz completion log
                EngagementType = EngagementType.Complete
            };
            await _patientEngagementRepository.CreateAsync(engagement);
            return true;
        }
    }
}