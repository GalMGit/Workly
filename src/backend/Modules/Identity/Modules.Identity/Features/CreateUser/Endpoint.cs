using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Identity.Application.Tags;
using Wolverine;
using Workly.Shared.Endpoint;
using Workly.Shared.ResultTools;
using Workly.Shared.Validation;

namespace Modules.Identity.Features.CreateUser;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (
                CreateUserRequest request,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result>(
                    new CreateUserCommand(
                        request), ct);

                return result.ToHttpResponse();
            })
            .AllowAnonymous()
            .WithTags(Tags.Identity)
            .AddEndpointFilter<FluentValidationFilter<CreateUserRequest>>();
    }
}