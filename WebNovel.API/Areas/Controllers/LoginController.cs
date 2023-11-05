using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Login;
using WebNovel.API.Areas.Models.Login.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class LoginController : BaseController
    {
        private readonly ILoginModel _loginModel;
        private readonly IServiceProvider _provider;
        public LoginController(ILoginModel loginModel, IServiceProvider provider) : base(provider)
        {
            _loginModel = loginModel;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromBody] UserEntity user)
        {
            try
            {
                return Ok(await _loginModel.Login(user.Username, user.Password));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost("login/refresh-token")]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenQuery token)
        {
            try
            {
                return Ok(await _loginModel.RefreshTokenAsync(token));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
    }
}