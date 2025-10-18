using ClickHealthBackend.Models;

namespace ClickHealthBackend.Repositories.Interfaces
{
    public interface IPatientInviteRepository
    {
        Task CreateAsync(PatientInvite invite);
        Task<PatientInvite> GetByInviteCodeAsync(string inviteCode);
        Task<bool> UpdateAsync(PatientInvite invite);
        Task<IEnumerable<PatientInvite>> GetAllAsync();
    }
}
