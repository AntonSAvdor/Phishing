using System.Threading.Tasks;
using PhishingSimulator.API.Models;

namespace PhishingSimulator.API.Services
{
    public interface IEmailService
    {
        Task SendPhishingEmailAsync(PhishingAttempt attempt);
    }
} 