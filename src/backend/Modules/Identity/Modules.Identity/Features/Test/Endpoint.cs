using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Workly.Shared.Endpoint;

namespace Modules.Identity.Features.Test;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("test", () => "test");
    }
}