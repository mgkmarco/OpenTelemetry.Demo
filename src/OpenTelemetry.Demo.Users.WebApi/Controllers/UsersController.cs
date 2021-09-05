using System;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Demo.Public.Contracts.DTOs;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using OpenTelemetry.Demo.Public.Contracts.Clients;
using OpenTelemetry.Demo.Public.Contracts.Models;
using OpenTelemetry.Demo.Public.Contracts.Options;

namespace OpenTelemetry.Demo.Users.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly ActivitySource Activity = new(nameof(UsersController));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private readonly DatasourceOptions _datasourceOptions;
        private readonly ILegislationsClient _legislationsClient;
        private readonly ILogger<UsersController> _logger;

        public UsersController([NotNull] IOptionsMonitor<DatasourceOptions> datasourceOptions, [NotNull] ILegislationsClient legislationsClient, ILogger<UsersController> logger)
        {
            _datasourceOptions = datasourceOptions.CurrentValue;
            _legislationsClient = legislationsClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<UserResponseDto> GetUser(int userId)
        {
            UserEntity userEntity;
            
            using (SqlConnection connection = new SqlConnection(
                _datasourceOptions.ConnectionString))
            {
                var response = await connection.QueryAsync<UserEntity>(
                    "SELECT user_Id AS UserID, username AS Username, email AS [Email] FROM dbo.USERS WITH(NOLOCK) WHERE user_id = @UserId", new {UserId = userId});

                userEntity = response.FirstOrDefault();
            }

            var loh = new LargeObject[2048];

            for (int i = 0; i < loh.Length; i++)
            {
                loh[i] = new LargeObject();
            }

            var LohDic = loh.ToDictionary(x => x.Id, o => o);
            var legislationsResponse = await _legislationsClient.GetLegislationAsync(userEntity.UserId);
            _logger.LogInformation($"LargeObject ID: {LohDic.First().Key}");

            using (var activity = Activity.StartActivity("GetUser", ActivityKind.Server))
            {
                AddActivityToHeader(activity);

                return new UserResponseDto
                {
                    UserId = userEntity.UserId,
                    Username = userEntity.Username
                };  
            }
        }

        private void AddActivityToHeader(Activity activity)
        {
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.queue", "sample");
        }
    }
}