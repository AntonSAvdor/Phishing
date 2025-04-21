using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using PhishingSimulator.API.Models;

namespace PhishingSimulator.API.Services
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoCollection<PhishingAttempt> _phishingAttempts;

        public MongoDBService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("PhishingSimulator");
            _phishingAttempts = database.GetCollection<PhishingAttempt>("PhishingAttempts");
        }

        public async Task<PhishingAttempt> CreatePhishingAttemptAsync(PhishingAttempt attempt)
        {
            await _phishingAttempts.InsertOneAsync(attempt);
            return attempt;
        }

        public async Task<PhishingAttempt> GetPhishingAttemptByIdAsync(string id)
        {
            return await _phishingAttempts.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<PhishingAttempt>> GetAllPhishingAttemptsAsync()
        {
            return await _phishingAttempts.Find(_ => true).ToListAsync();
        }

        public async Task<bool> UpdatePhishingAttemptClickStatusAsync(string id)
        {
            var update = Builders<PhishingAttempt>.Update
                .Set(a => a.IsClicked, true)
                .Set(a => a.ClickedAt, DateTime.UtcNow);

            var result = await _phishingAttempts.UpdateOneAsync(a => a.Id == id, update);
            return result.ModifiedCount > 0;
        }
    }
} 