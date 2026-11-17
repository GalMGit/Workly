using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;
using Workly.Shared.Endpoint;
using Workly.Shared.ResultTools;
using Workly.Shared.Validation;

namespace Modules.Identity.Features.ConfirmEmail;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/email/confirm", async (
                ConfirmEmailRequest request,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus.InvokeAsync<Result>(
                    new ConfirmEmailCommand(
                        request), ct);

                return result.ToHttpResponse();
            })
            .WithTags(Tags.Identity)
            .AllowAnonymous()
            .AddEndpointFilter<FluentValidationFilter<ConfirmEmailRequest>>();
    }
}