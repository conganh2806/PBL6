using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Bundles;
using WebNovel.API.Areas.Models.Bundles.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/bundles")]
    [ApiController]
    public class BundleController : BaseController
    {
        private readonly IBundleModel _bundleModel;
        private readonly IServiceProvider _provider;
        public BundleController(IBundleModel bundleModel, IServiceProvider provider) : base(provider)
        {
            _bundleModel = bundleModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BundleDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _bundleModel.GetListBundle());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] BundleCreateEntity bundle)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _bundleModel.AddBundle(bundle);
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

        [HttpPut]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] BundleUpdateEntity bundle)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _bundleModel.UpdateBundle(bundle);
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
        public async Task<IActionResult> Delete([FromBody] BundleDeleteEntity bundle)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _bundleModel.RemoveBundle(bundle);
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
