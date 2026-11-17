using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Identity.Contracts.Events;
using Modules.Profiles.Contracts.Events;
using Modules.Profiles.Domain;
using Modules.Profiles.Infrastructure.Persistence.Context;
using Wolverine;
using Wolverine.Attributes;

namespace Modules.Profiles.Features.OnUserCreated;

[Transactional(typeof(ProfilesDbContext))]
public sealed class UserCreatedHandler(
    ProfilesDbContext context,
    IMessageBus bus,
    ILogger<UserCreatedHandler> logger)
{
    public async Task Handle(
        UserCreated @event,
        CancellationToken ct)
    {
        var exists = await context.Profiles
            .AnyAsync(x => 
                x.Id == @event.UserId, ct);

        if (exists)
        {
            logger.LogInformation(
                "Profile for user {UserId} already exists, skipping",
                @event.UserId);
            return;
        }

        var profile = new Profile
        {
            Id = @event.UserId,
            Username = @event.Username,
            CreatedAt = DateTime.UtcNow
        };

        await context.Profiles.AddAsync(
            profile, ct);
        
        await bus.PublishAsync(
            new UserProfileCreated(
                profile.Id,
                profile.Username));

        logger.LogInformation(
            "Profile created for user {UserId}",
            @event.UserId);
    }
}
