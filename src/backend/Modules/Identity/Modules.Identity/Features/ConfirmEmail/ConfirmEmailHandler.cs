using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions.Cache;
using Modules.Identity.Application.Errors;
using Modules.Identity.Application.Names;
using Modules.Identity.Contracts.Events;
using Modules.Identity.Domain;
using Modules.Identity.Infrastructure.Persistence.Context;
using Wolverine;
using Wolverine.Attributes;
using Workly.Shared.ResultTools;

namespace Modules.Identity.Features.ConfirmEmail;

[Transactional(typeof(IdentityDbContext))]
public sealed class ConfirmEmailHandler(
    IdentityDbContext context,
    ICacheService cacheService,
    IMessageBus bus)
{
    public async Task<Result> Handle(
        ConfirmEmailCommand command,
        CancellationToken ct)
    {
        var email = command.Request.Email.Trim().ToLowerInvariant();
        
        var cacheKey = $"temp_user:{email}";
        
        var tempUser = await cacheService.GetAsync<TempUser>(
            cacheKey, ct);

        if (tempUser is null)
            return Result.Failure(
                UserErrors.ConfirmationCodeNotFound);

        if (tempUser.ConfirmationCode != command.Request.Code)
            return Result.Failure(
                UserErrors.ConfirmationCodeInvalid);

        if (tempUser.CodeExpiresAt < DateTime.UtcNow)
            return Result.Failure(
                UserErrors.ConfirmationCodeExpired);

        var normalizedEmail = tempUser.Email.Trim().ToLowerInvariant();
        var normalizedUsername = tempUser.Username.Trim().ToLowerInvariant();

        if (await context.Users.AnyAsync(
                x => x.Email.ToLower() == normalizedEmail, ct))
            return Result.Failure(
                UserErrors.EmailExists);

        if (await context.Users.AnyAsync(
                x => x.Username.ToLower() == normalizedUsername, ct))
            return Result.Failure(
                UserErrors.UsernameExists);
        
        var seekerRole = await context.Roles
            .SingleAsync(x =>
                x.Name == RoleNames.Seeker, ct);

        var user = new User
        {
            Id = tempUser.Id,
            Username = tempUser.Username,
            CreatedAt = tempUser.CreatedAt,
            Email = tempUser.Email,
            PasswordHash = tempUser.PasswordHash,
            Roles = [seekerRole]
        };

        await context.Users.AddAsync(
            user, ct);

        await cacheService.RemoveAsync(
            cacheKey, ct);

        await bus.PublishAsync(
            new UserCreatedEvent(
                user.Id,
                tempUser.Username));
        
        return Result.Success();
    }
}