using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Identity.Application.Abstractions.Auth;
using Modules.Identity.Domain;
using Modules.Identity.Infrastructure.Persistence.Context;

namespace Modules.Identity.Infrastructure.Persistence.Seed;

public static class IdentitySeeder
{
    private static readonly IReadOnlyDictionary<string, string[]> RolePermissions =
        new Dictionary<string, string[]>
        {
            [Roles.SuperAdmin] = [.. Permissions.All],

            [Roles.Admin] =
            [
                Permissions.Users.Read,
                Permissions.Users.Ban,
                Permissions.Users.Delete,
                Permissions.Companies.Manage,
                Permissions.Vacancies.Moderate,
                Permissions.Resumes.Moderate,
                Permissions.Complaints.Resolve,
                Permissions.Reports.View
            ],

            [Roles.Moderator] =
            [
                Permissions.Users.Read,
                Permissions.Users.Ban,
                Permissions.Vacancies.Moderate,
                Permissions.Resumes.Moderate,
                Permissions.Complaints.Resolve
            ],

            [Roles.Employer] =
            [
                Permissions.Companies.Manage,
                Permissions.Vacancies.Manage,
                Permissions.Responses.Manage
            ],

            [Roles.Seeker] =
            [
                Permissions.Resumes.Manage,
                Permissions.Responses.Manage
            ]
        };

    public static async Task SeedAsync(
        IdentityDbContext db,
        AdminOptions adminOptions,
        IPasswordHasher passwordHasher,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var permissions = await EnsurePermissionsAsync(
            db,
            cancellationToken);

        var roles = await EnsureRolesAsync(
            db,
            permissions,
            cancellationToken);

        await EnsureSuperAdminAsync(
            db,
            roles[Roles.SuperAdmin],
            adminOptions,
            passwordHasher,
            logger,
            cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Dictionary<string, Permission>> EnsurePermissionsAsync(
        IdentityDbContext db,
        CancellationToken cancellationToken)
    {
        var permissions = await db.Permissions
            .ToDictionaryAsync(p => p.Name, cancellationToken);

        foreach (var name in Permissions.All)
        {
            if (permissions.ContainsKey(name))
                continue;

            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            db.Permissions.Add(permission);
            permissions[name] = permission;
        }

        return permissions;
    }

    private static async Task<Dictionary<string, Role>> EnsureRolesAsync(
        IdentityDbContext db,
        IReadOnlyDictionary<string, Permission> permissions,
        CancellationToken cancellationToken)
    {
        var roles = await db.Roles
            .Include(r => r.Permissions)
            .ToDictionaryAsync(r => r.Name, cancellationToken);

        foreach (var (roleName, permissionNames) in RolePermissions)
        {
            if (!roles.TryGetValue(roleName, out var role))
            {
                role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName
                };

                db.Roles.Add(role);
                roles[roleName] = role;
            }

            var currentPermissions = role.Permissions
                .Select(p => p.Name)
                .ToHashSet(StringComparer.Ordinal);

            foreach (var permissionName in permissionNames)
            {
                if (!currentPermissions.Add(permissionName))
                    continue;

                role.Permissions.Add(permissions[permissionName]);
            }
        }

        return roles;
    }

    private static async Task EnsureSuperAdminAsync(
        IdentityDbContext db,
        Role superAdminRole,
        AdminOptions adminOptions,
        IPasswordHasher passwordHasher,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(adminOptions.Username) ||
            string.IsNullOrWhiteSpace(adminOptions.Email) ||
            string.IsNullOrWhiteSpace(adminOptions.Password))
        {
            logger.LogWarning(
                "Identity:Admin is not configured. Super admin user was not seeded.");

            return;
        }

        var admin = await db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(
                u => u.Email == adminOptions.Email ||
                     u.Username == adminOptions.Username,
                cancellationToken);

        if (admin is null)
        {
            admin = new User
            {
                Id = Guid.NewGuid(),
                Username = adminOptions.Username,
                Email = adminOptions.Email,
                PasswordHash = passwordHasher.GenerateHash(
                    adminOptions.Password),
                CreatedAt = DateTime.UtcNow
            };

            db.Users.Add(admin);

            logger.LogInformation(
                "Seeded super admin user {Email}.",
                adminOptions.Email);
        }
        else
        {
            logger.LogInformation(
                "Super admin user {Email} already exists.",
                adminOptions.Email);
        }

        if (admin.Roles.All(r => r.Name != Roles.SuperAdmin))
            admin.Roles.Add(superAdminRole);
    }
}
