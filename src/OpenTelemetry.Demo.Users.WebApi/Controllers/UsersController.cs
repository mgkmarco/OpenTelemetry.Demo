using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Demo.Public.Contracts.DTOs;
using System.Diagnostics;

namespace OpenTelemetry.Demo.Users.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly ActivitySource Activity = new(nameof(UsersController));
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public UserResponseDto GetUser(int userId)
        {
            using (var activity = Activity.StartActivity("GetUser", ActivityKind.Server))
            {
                AddActivityToHeader(activity);

                return new UserResponseDto
                {
                    Username = "Marco",
                    UserId = 666
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