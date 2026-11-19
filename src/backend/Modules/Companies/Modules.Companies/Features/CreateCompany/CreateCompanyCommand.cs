namespace Modules.Companies.Features.CreateCompany;

public sealed record CreateCompanyCommand(
    CreateCompanyRequest Request,
    Guid UserId);
