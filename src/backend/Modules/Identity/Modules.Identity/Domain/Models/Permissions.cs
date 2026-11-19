namespace Modules.Identity.Domain.Models;

public static class Permissions
{
    public static class Users
    {
        public const string Read = "users.read";
        public const string Ban = "users.ban";
        public const string Delete = "users.delete";
    }

    public static class Staff
    {
        public const string Manage = "staff.manage";
    }

    public static class Vacancies
    {
        public const string Moderate = "vacancies.moderate";
        public const string Manage = "vacancies.manage";
    }

    public static class Resumes
    {
        public const string Moderate = "resumes.moderate";
        public const string Manage = "resumes.manage";
    }

    public static class Complaints
    {
        public const string Resolve = "complaints.resolve";
    }

    public static class Companies
    {
        public const string Manage = "companies.manage";
    }

    public static class Responses
    {
        public const string Manage = "responses.manage";
    }

    public static class Reports
    {
        public const string View = "reports.view";
    }

    public static readonly IReadOnlyList<string> All =
    [
        Users.Read,
        Users.Ban,
        Users.Delete,
        Staff.Manage,
        Vacancies.Moderate,
        Vacancies.Manage,
        Resumes.Moderate,
        Resumes.Manage,
        Complaints.Resolve,
        Companies.Manage,
        Responses.Manage,
        Reports.View
    ];
}
