using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Login;
using WebNovel.API.Core.Services;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Controllers;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Commons;
using WebNovel.API.Areas.Models.Login.Schemas;


namespace WebNovel.API.Areas.Controllers
{
    public class LoginWithSocialAccountController : BaseController
    {
        private readonly ILoginModel _loginModel;
        private readonly ILogService _logService;

        public LoginWithSocialAccountController(ILoginModel loginModel, IServiceProvider provider)
        {
            ILogService logService = provider.GetService<ILogService>();
            _loginModel = loginModel ?? throw new ArgumentNullException(nameof(loginModel));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpPost("google")]
        public async Task<ActionResult<TokenResponse>> CheckTokenAsync([FromForm]GoogleOauthUser googleUser)
        {
            return await _loginModel.GetGoogleUserTokenAsync(googleUser);
        }
    }
}