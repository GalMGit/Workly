using Microsoft.EntityFrameworkCore;

namespace Workly.Api.Infrastructure.Messaging;

public sealed class MessagingDbContext(
    DbContextOptions<MessagingDbContext> options)
    : DbContext(options);
