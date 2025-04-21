using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PhishingSimulator.API.Models;
using PhishingSimulator.API.Services;

namespace PhishingSimulator.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhishingController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IMongoDBService _mongoDBService;

        public PhishingController(IEmailService emailService, IMongoDBService mongoDBService)
        {
            _emailService = emailService;
            _mongoDBService = mongoDBService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendPhishingEmail([FromBody] PhishingAttempt attempt)
        {
            try
            {
                attempt.Id = Guid.NewGuid().ToString();
                attempt.CreatedAt = DateTime.UtcNow;
                attempt.IsClicked = false;
                attempt.TrackingLink = $"{Request.Scheme}://{Request.Host}/api/phishing/track/{attempt.Id}";

                // Save to MongoDB
                await _mongoDBService.CreatePhishingAttemptAsync(attempt);

                // Send email
                await _emailService.SendPhishingEmailAsync(attempt);

                return Ok(new { message = "Phishing email sent successfully", attemptId = attempt.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send phishing email", error = ex.Message });
            }
        }

        [HttpGet("track/{id}")]
        public async Task<IActionResult> TrackClick(string id)
        {
            try
            {
                var success = await _mongoDBService.UpdatePhishingAttemptClickStatusAsync(id);
                if (success)
                {
                    return Ok(new { message = "Click tracked successfully", attemptId = id });
                }
                return NotFound(new { message = "Phishing attempt not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to track click", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<PhishingAttempt>>> GetAllAttempts()
        {
            try
            {
                var attempts = await _mongoDBService.GetAllPhishingAttemptsAsync();
                return Ok(attempts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve phishing attempts", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PhishingAttempt>> GetAttemptById(string id)
        {
            try
            {
                var attempt = await _mongoDBService.GetPhishingAttemptByIdAsync(id);
                if (attempt == null)
                {
                    return NotFound(new { message = "Phishing attempt not found" });
                }
                return Ok(attempt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve phishing attempt", error = ex.Message });
            }
        }
    }
} 