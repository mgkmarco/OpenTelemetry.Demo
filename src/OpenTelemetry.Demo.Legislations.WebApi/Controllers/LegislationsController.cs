using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Demo.Legislations.WebApi.Extensions;
using OpenTelemetry.Demo.Public.Contracts.DTOs;
using StackExchange.Redis;

namespace OpenTelemetry.Demo.Legislations.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LegislationsController : ControllerBase
    {
        private static readonly ActivitySource Activity = new(nameof(LegislationsController));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        
        private readonly IDatabase _redis;
        private readonly IAdvancedBus _bus;
        private readonly ILogger<LegislationsController> _logger;
        private readonly string _sampleMarketSet;

        public LegislationsController([NotNull] IDatabase redis, [NotNull] IAdvancedBus bus, ILogger<LegislationsController> logger)
        {
            _bus = bus;
            _redis = redis;
            _logger = logger;
            _sampleMarketSet = System.IO.File.ReadAllText("Messaging/SampleMarketSet.txt");
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

            var exchange = new Exchange("ti.testTrdCluster", ExchangeType.Topic);
            
            var body = Encoding.UTF8.GetBytes(_sampleMarketSet);

            for (int i = 0; i < 20; i++)
            {
                using (var activity = Activity.StartActivity(nameof(_bus.PublishAsync), ActivityKind.Producer))
                {
                    var tags = new Dictionary<string, object>
                    {
                        { "messaging.system", "rabbitmq" },
                        { "messaging.destination_kind", "exchange" },
                        { "messaging.rabbitmq.exchange", exchange.Name },
                        { "messaging.rabbitmq.exchange-type", exchange.Type },
                    };

                    var messageProperties = new MessageProperties();
                    activity.AddActivityToHeader(Propagator, tags, messageProperties, _logger);

                    await _bus.PublishAsync(exchange, "#", true, messageProperties, body);
                }
            }

            return new LegislationsResponseDto
            {
                LegislationId = 1,
                LegislationName = "MGA"
            };
        }
    }
}