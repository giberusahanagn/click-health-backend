using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Data;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClickHealthBackend.Repositories.Implementations
{
    public class PatientInviteRepository : IPatientInviteRepository
    {
        private readonly IMongoCollection<PatientInvite> _invites;

        public PatientInviteRepository(MongoDbContext context)
        {
            _invites = context.PatientInvite;
        }

        public async Task CreateAsync(PatientInvite invite) =>
            await _invites.InsertOneAsync(invite);

        public async Task<PatientInvite> GetByInviteCodeAsync(string inviteCode) =>
            await _invites.Find(i => i.InviteCode == inviteCode).FirstOrDefaultAsync();

        public async Task<bool> UpdateAsync(PatientInvite invite)
        {
            var result = await _invites.ReplaceOneAsync(i => i.Id == invite.Id, invite);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<PatientInvite>> GetAllAsync() =>
            await _invites.Find(_ => true).ToListAsync();
    }
}