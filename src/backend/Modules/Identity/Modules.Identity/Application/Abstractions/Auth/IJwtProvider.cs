using Modules.Identity.Domain;
using Modules.Identity.Domain.Models;

namespace Modules.Identity.Application.Abstractions.Auth;

public interface IJwtProvider
{
    string GenerateRefreshToken();
    string GenerateToken(User user);
}