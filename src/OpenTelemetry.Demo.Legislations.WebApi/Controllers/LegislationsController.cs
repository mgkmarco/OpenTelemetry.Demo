using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Demo.Public.Contracts.DTOs;
using StackExchange.Redis;

namespace OpenTelemetry.Demo.Legislations.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LegislationsController : ControllerBase
    {
        private readonly IDatabase _redis;
        private readonly ILogger<LegislationsController> _logger;

        public LegislationsController([NotNull] IDatabase redis, ILogger<LegislationsController> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        [HttpGet("/user/{userId}")]
        public async Task<LegislationsResponseDto> GetAsync(int userId)
        {
            Random r = new Random();
            var expiryKeys = r.Next(1, 100);

            for (int i = 0; i < expiryKeys; i++)
            {
                var key = $"userid_{r.Next()}";
                var expiry = r.Next(1, 10);
                await _redis.StringSetAsync(key, Guid.NewGuid().ToString(), TimeSpan.FromMinutes(expiry));
                var userCacheEntry = await _redis.StringGetAsync(key);
            }
            
            var nonExpiryKeys = r.Next(1, 100);

            for (int i = 0; i < nonExpiryKeys; i++)
            {
                var key = $"userid_{r.Next()}";
                await _redis.StringSetAsync(key, Guid.NewGuid().ToString());
                var userCacheEntry = await _redis.StringGetAsync(key);
            }
            
            return new LegislationsResponseDto
            {
                LegislationId = 1,
                LegislationName = "MGA"
            };
        }
    }
}