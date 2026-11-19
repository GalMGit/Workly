namespace Modules.Identity.Domain.Models;

public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Moderator = "Moderator";
    public const string Employer = "Employer";
    public const string Seeker = "Seeker";

    public static readonly IReadOnlyList<string> All =
    [
        SuperAdmin,
        Admin,
        Moderator,
        Employer,
        Seeker
    ];
}
