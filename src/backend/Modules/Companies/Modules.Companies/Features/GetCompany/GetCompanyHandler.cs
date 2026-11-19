using Microsoft.EntityFrameworkCore;
using Modules.Companies.Application.Dtos;
using Modules.Companies.Application.Errors;
using Modules.Companies.Infrastructure.Persistence.Context;
using Workly.Shared.ResultTools;

namespace Modules.Companies.Features.GetCompany;

public sealed class GetCompanyHandler(
    CompaniesDbContext context)
{
    public async Task<Result<CompanyDetails>> Handle(
        GetCompanyQuery query,
        CancellationToken ct)
    {
        var company = await context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == query.CompanyId &&
                x.DeletedAt == null, ct);

        if (company is null)
            return Result<CompanyDetails>.Failure(
                CompanyErrors.NotFound);

        return Result<CompanyDetails>.Success(
            company.ToDetails());
    }
}
