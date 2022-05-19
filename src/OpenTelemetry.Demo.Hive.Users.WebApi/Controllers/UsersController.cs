using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Demo.Public.Contracts.Clients;
using OpenTelemetry.Demo.Public.Contracts.DTOs;
using OpenTelemetry.Demo.Public.Contracts.Models;
using OpenTelemetry.Demo.Public.Contracts.Options;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTelemetry.Demo.Hive.Users.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly ActivitySource Activity = new("OT.Demo.Hive.Users");
        private readonly DatasourceOptions _datasourceOptions;
        private readonly ILegislationsClient _legislationsClient;

        public UsersController([NotNull] IOptionsMonitor<DatasourceOptions> datasourceOptions,
            [NotNull] ILegislationsClient legislationsClient)
        {
            _datasourceOptions = datasourceOptions.CurrentValue;
            _legislationsClient = legislationsClient;
        }

        [HttpGet]
        public async Task<UserResponseDto> GetUser(int userId)
        {
            UserEntity userEntity;

            using (SqlConnection connection = new SqlConnection(
                _datasourceOptions.ConnectionString))
            {
                var response = await connection.QueryAsync<UserEntity>("[dbo].[sp_GetUser]", new { UserId = userId }, commandType: CommandType.StoredProcedure);

                userEntity = response.FirstOrDefault();
            }

            using (var activity = Activity.StartActivity("GetUser", ActivityKind.Internal))
            {
                //Simulate Large Object on the heap for GEN2 promotions
                var loh = new LargeObject[2048];

                for (int i = 0; i < loh.Length; i++)
                {
                    loh[i] = new LargeObject();
                }

                using (var activity2 = Activity.StartActivity("this-is-something-else", ActivityKind.Internal))
                {
                    for (int i = 0; i < loh.Length; i++)
                    {
                        loh[i] = new LargeObject();
                    }
                }

                    var LohDic = loh.ToDictionary(x => x.Id, o => o);
                var legislationsResponse = await _legislationsClient.GetLegislationAsync(userEntity.UserId);

                activity?.SetTag("internal.method", "GetUser");
                activity?.SetTag("internal.method.scope", "LOH GC Simulation");

                return new UserResponseDto
                {
                    UserId = userEntity.UserId,
                    Username = userEntity.Username
                };
            }
        }
    }
}
