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
            var key = $"userid_{userId}";
            await _redis.StringSetAsync(key, Guid.NewGuid().ToString());
            var userCacheEntry = await _redis.StringGetAsync(key);
            
            return new LegislationsResponseDto
            {
                LegislationId = 1,
                LegislationName = userCacheEntry.ToString()
            };
        }
    }
}