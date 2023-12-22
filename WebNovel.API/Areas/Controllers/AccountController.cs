using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Areas.Models.Accounts.Schemas;
using WebNovel.API.Areas.Models.Login;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IAccountModel _accountModel;
        private readonly IServiceProvider _provider;
        private readonly ILoginModel _loginModel;
        public AccountController(ILoginModel loginModel, IAccountModel accountModel, IServiceProvider provider) : base(provider)
        {
            _accountModel = accountModel;
            _loginModel = loginModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Search([FromQuery] SearchCondition condition)
        {
            try
            {
                return Ok(await _accountModel.GetListAccount(condition));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(await _accountModel.GetAccount(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] AccountCreateEntity account)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _accountModel.AddAccount(account);
                    if (response.Code != CodeResponse.OK)
                    {
                        return BadRequest(response);
                    }
                }
                else
                {
                    response.Code = CodeResponse.NOT_VALIDATE;
                    return Ok(response);
                }

                return Ok(await _loginModel.Login(account.Email, account.Password));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] AccountUpdateEntity account)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _accountModel.UpdateAccount(account);
                }
                else
                {
                    response.Code = CodeResponse.NOT_VALIDATE;
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] AccountDeleteEntity account)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _accountModel.RemoveAccount(account);
                }
                else
                {
                    response.Code = CodeResponse.NOT_VALIDATE;
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("accounts-info-admin")]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetNumberAccount()
        {
            try
            {
                return Ok(await _accountModel.GetNumberAccount());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
    }
}