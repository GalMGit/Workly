using Microsoft.AspNetCore.Routing;

namespace Workly.Shared.Endpoint;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}