using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.UpdatedFees;
using WebNovel.API.Areas.Models.UpdatedFees.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/updatedfees")]
    [ApiController]
    public class UpdatedFeeController : BaseController
    {
        private readonly IUpdatedFeeModel _updatedFeeModel;
        private readonly IServiceProvider _provider;
        public UpdatedFeeController(IUpdatedFeeModel updatedFeeModel, IServiceProvider provider) : base(provider)
        {
            _updatedFeeModel = updatedFeeModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UpdatedFeeDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _updatedFeeModel.GetListUpdatedFee());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("get-active-fee")]
        [ProducesResponseType(typeof(UpdatedFeeDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                return Ok(await _updatedFeeModel.GetActiveFee());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] UpdatedFeeCreateUpdateEntity updatedFee)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _updatedFeeModel.AddUpdatedFee(updatedFee);
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