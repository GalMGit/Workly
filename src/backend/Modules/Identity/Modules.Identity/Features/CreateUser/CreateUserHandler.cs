using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions.Auth;
using Modules.Identity.Application.Abstractions.Cache;
using Modules.Identity.Application.Errors;
using Modules.Identity.Contracts.Events;
using Modules.Identity.Domain;
using Modules.Identity.Infrastructure.Persistence.Context;
using Wolverine;
using Workly.Shared.ResultTools;

namespace Modules.Identity.Features.CreateUser;

public sealed class CreateUserHandler(
    IdentityDbContext context,
    ICacheService cacheService,
    IMessageBus bus,
    IPasswordHasher passwordHasher)
{
    public async Task<Result> Handle(
        CreateUserCommand command,
        CancellationToken ct)
    {
        var email = command.Request.Email.Trim();
        var username = command.Request.Username.Trim();

        var normalizedEmail = email.ToLowerInvariant();
        var normalizedUsername = username.ToLowerInvariant();
        
        var cacheKey = $"temp_user:{normalizedEmail}";
        
        if (await cacheService.ExistsAsync(
                cacheKey, ct))
            return Result.Failure(
                UserErrors.RegistrationPending);
        
        var emailExists = await context.Users
            .AnyAsync(x => 
                x.Email.ToLower() == normalizedEmail, ct);

        if (emailExists)
            return Result.Failure(
                UserErrors.EmailExists);

        var usernameExists = await context.Users
            .AnyAsync(x => 
                x.Username.ToLower() == normalizedUsername, ct);
        
        if (usernameExists)
            return Result.Failure(
                UserErrors.UsernameExists);
        
        var confirmationCode =
            RandomNumberGenerator
                .GetInt32(10000, 100000)
                .ToString();
        
        var tempUser = new TempUser
        {
            Id = Guid.CreateVersion7(),
            Username = username,
            Email = email,
            PasswordHash = passwordHasher.GenerateHash(
                command.Request.Password),
            CreatedAt = DateTime.UtcNow,
            ConfirmationCode = confirmationCode,
            CodeExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        
        await cacheService.SetAsync(
            cacheKey,
            tempUser,
            TimeSpan.FromMinutes(5),
            ct);
        
        await bus.PublishAsync(
            new UserRegistrationStarted(
                tempUser.Email,
                tempUser.Username,
                tempUser.ConfirmationCode));
        
        return Result.Success();
    }
}