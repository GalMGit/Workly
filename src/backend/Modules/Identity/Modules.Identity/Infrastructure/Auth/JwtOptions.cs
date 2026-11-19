namespace Modules.Identity.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string SecretKey { get; init; } = string.Empty;
    public int Expires { get; init; }
}