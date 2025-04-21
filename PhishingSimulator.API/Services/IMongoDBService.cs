using System.Collections.Generic;
using System.Threading.Tasks;
using PhishingSimulator.API.Models;

namespace PhishingSimulator.API.Services
{
    public interface IMongoDBService
    {
        Task<PhishingAttempt> CreatePhishingAttemptAsync(PhishingAttempt attempt);
        Task<PhishingAttempt> GetPhishingAttemptByIdAsync(string id);
        Task<List<PhishingAttempt>> GetAllPhishingAttemptsAsync();
        Task<bool> UpdatePhishingAttemptClickStatusAsync(string id);
    }
} 