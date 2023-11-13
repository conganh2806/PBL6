using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebNovel.API.Areas.Models.Accounts.Schemas;
using WebNovel.API.Areas.Models.Novels;
using WebNovel.API.Areas.Models.Novels.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/novel")]
    [ApiController]
    public class NovelController : BaseController
    {
        private readonly INovelModel _novelModel;
        private readonly IServiceProvider _provider;

        public NovelController(INovelModel novelModel, IServiceProvider provider) : base(provider)
        {
            _novelModel = novelModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(NovelDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search([FromQuery] SearchCondition searchCondition)
        {
            try
            {
                return Ok(await _novelModel.GetListNovel(searchCondition));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NovelDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(await _novelModel.GetNovelAsync(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("GenreId={GenreId}")]
        [ProducesResponseType(typeof(NovelDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetListNovelByGenreId([FromRoute] long GenreId)
        {
            try
            {
                return Ok(await _novelModel.GetListNovelByGenreId(GenreId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("AccountId={AccountId}")]
        [ProducesResponseType(typeof(NovelDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetListNovelByAccountId([FromRoute] string AccountId)
        {
            try
            {
                return Ok(await _novelModel.GetListNovelByAccountId(AccountId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] NovelCreateEntity novel)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _novelModel.AddNovel(novel);
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
        public async Task<IActionResult> Update([FromForm] NovelUpdateEntity novel)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _novelModel.UpdateNovel(novel);
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