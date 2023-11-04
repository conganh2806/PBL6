using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Core.Models;
using WebNovel.API.Core.Services;

namespace WebNovel.API.Areas.Models.Login
{
    public interface ILoginModel
    {
        Task<TokenResponse> Login(string email, string password);
        Task<TokenResponse> RefreshTokenAsync(RefreshTokenQuery request);
    }
    public class LoginModel : BaseModel, ILoginModel
    {
        private readonly IAccountModel _accountModel;
        private readonly ITokenService  _tokenService;

        private readonly ILogger<ILoginModel> _logger;
        private string _className = "";
        public LoginModel (IServiceProvider provider, ILogger<ILoginModel> logger) : base(provider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _className = GetType().Name;
        }
        
        public async Task<TokenResponse> Login(string email, string password)
        {
            var userId = await _accountModel.LoginWithPasswordAsync(email, password);

            return string.IsNullOrEmpty(userId)
            ? throw new UnauthorizedAccessException("Authentication Failed.")
            : await _tokenService.GetTokenAsync(userId);
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenQuery request) => await _tokenService.RefreshTokenAsync(request);
    }
}