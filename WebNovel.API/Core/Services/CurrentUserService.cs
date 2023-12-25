using System.Security.Claims;

namespace WebNovel.API.Core.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? IpAddress { get; }
    }
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public string? UserId => httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? IpAddress => httpContextAccessor?.HttpContext?.Connection?.LocalIpAddress?.ToString();
    }
}
