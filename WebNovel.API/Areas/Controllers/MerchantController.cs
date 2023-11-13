using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Merchant;
using WebNovel.API.Areas.Models.Merchant.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/merchants")]
    [ApiController]
    public class MerchantController : BaseController
    {
        private readonly IMerchantModel _merchantModel;
        private readonly IServiceProvider _provider;
        public MerchantController(IMerchantModel merchantModel, IServiceProvider provider) : base(provider)
        {
            _merchantModel = merchantModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(MerchantDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _merchantModel.GetListMerchant());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MerchantDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(await _merchantModel.GetMerchant(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        /// <summary>
        /// Create merchant
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>
        /// 
        ///     POST /merchants
        ///     {
        ///         "MerchantName" : "Website bán hàng A",
        ///         "MerchantWebLink" : "https://webbanhang.com",
        ///         "MerchantIpnUrl" : "https://webbanhang.com/ipn",
        ///         "MerchantReturnUrl" : "https://webbanhang.com/payment/return"
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] MerchantCreateUpdateEntity merchant)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _merchantModel.AddMerchant(merchant);
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
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] MerchantCreateUpdateEntity merchant)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _merchantModel.UpdateMerchant(id, merchant);
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
