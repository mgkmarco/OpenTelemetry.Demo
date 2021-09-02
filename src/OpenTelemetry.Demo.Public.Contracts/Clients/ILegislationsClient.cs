using System.Threading.Tasks;
using OpenTelemetry.Demo.Public.Contracts.DTOs;
using Refit;

namespace OpenTelemetry.Demo.Public.Contracts.Clients
{
    public interface ILegislationsClient
    {
        [Get("/user/{userId}")]
        Task<LegislationsResponseDto> GetLegislationAsync(int userId);
    }
}