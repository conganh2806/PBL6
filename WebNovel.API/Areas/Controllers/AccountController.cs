using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Accounts;
using WebNovel.API.Areas.Models.Accounts.Schemas;
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
        public AccountController(IAccountModel accountModel, IServiceProvider provider) : base(provider)
        {
            _accountModel = accountModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
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
        public async Task<IActionResult> GetDetail([FromRoute]long Id)
        {
            try
            {
                return Ok(_accountModel.GetAccount(Id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] AccountCreateUpdateEntity account)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _accountModel.AddAccount(account);
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] AccountCreateUpdateEntity account)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _accountModel.UpdateAccount(id, account);
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
    }
}