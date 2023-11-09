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

        [HttpGet("login-with-google", Name = "GoogleLogin")]
        public IActionResult GoogleLogin(string url)
        {
            var redirectUrl = string.IsNullOrEmpty(url) ? Url.Action(nameof(SignInGoogle), "LoginWithSocialAccount") : url;
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback", Name = "GoogleCallback")]
        public async Task<IActionResult> SignInGoogle(string beforeLink)
        {
            try
            {
                var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var email = authenticateResult.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var response = await _loginModel.LoginWithGoogle(email);

                SetCookiesResponse(response);
                return Redirect("/");
            }
            catch (Exception e)
            {
                await _logService.SaveLogException(e);
                return RedirectToAction("Error", "Error", new { error = e.Message });
            }
        }

        private void SetCookiesResponse(ResponseInfo response)
        {
            Response.Cookies.Append("token", Security.Base64Encode(response.Data["token"]), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(10),
                Secure = true,
                HttpOnly = true
            });

            Response.Cookies.Append("refreshToken", Security.Base64Encode(response.Data["refreshToken"]), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(10),
                Secure = true,
                HttpOnly = true
            });

            var date = DateTime.UtcNow.AddMinutes(int.Parse(response.Data["expires"]));

            Response.Cookies.Append("expires", date.ToUniversalTime().ToString("R"), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(10),
                Secure = true,
                HttpOnly = true
            });

            Response.Cookies.Delete("guestUser");
        }
    }
}