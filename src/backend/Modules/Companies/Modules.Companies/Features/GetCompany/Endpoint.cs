using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Modules.Companies.Application.Dtos;
using Modules.Companies.Application.Endpoints;
using Modules.Companies.Application.Tags;
using Wolverine;
using Workly.Shared.Endpoint;
using Workly.Shared.ResultTools;

namespace Modules.Companies.Features.GetCompany;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("companies/{companyId:guid}", async (
                Guid companyId,
                IMessageBus bus,
                CancellationToken ct) =>
            {
                var result = await bus
                    .InvokeAsync<Result<CompanyDetails>>(
                        new GetCompanyQuery(companyId), ct);

                return result.ToHttpResponse();
            })
            .AllowAnonymous()
            .WithName(EndpointNames.GetCompanyById)
            .WithTags(Tags.Companies);
    }
}
