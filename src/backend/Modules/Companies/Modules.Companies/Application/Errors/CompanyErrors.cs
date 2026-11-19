using Workly.Shared.ResultTools;

namespace Modules.Companies.Application.Errors;

public static class CompanyErrors
{
    public static readonly Error NotFound =
        Error.NotFound(
            "company.not_found", 
            "Компания не найдена.");
}
