using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CookingAILand;

public interface IUserContextService
{
    ClaimsPrincipal User { get; }
    Guid? GetUserId { get; }
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

    public Guid? GetUserId =>
        User is null ? null : (Guid?)Guid.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
}