using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Companies.Application.Dtos;
using Modules.Companies.Application.Endpoints;
using Modules.Companies.Application.Tags;
using Wolverine;
using Workly.Shared.Endpoint;
using Workly.Shared.ResultTools;
using Workly.Shared.Validation;

namespace Modules.Companies.Features.CreateCompany;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("companies", async (
                CreateCompanyRequest request,
                ClaimsPrincipal user,
                LinkGenerator links,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus
                    .InvokeAsync<Result<CompanyDetails>>(
                        new CreateCompanyCommand(
                            request,
                            user.GetUserId()), ct);

                if (result.IsFailure)
                    return result.ToHttpResponse();

                var company = result.Value!;

                var location = links.GetPathByName(
                    EndpointNames.GetCompanyById,
                    new { companyId = company.Id });

                return Results.Created(
                    location,
                    company);
            })
            .RequireAuthorization()
            .WithTags(Tags.Companies)
            .AddEndpointFilter<FluentValidationFilter<CreateCompanyRequest>>();
    }
}
