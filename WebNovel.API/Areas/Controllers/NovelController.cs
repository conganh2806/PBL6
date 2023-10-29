using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebNovel.API.Areas.Models.Novels;
using WebNovel.API.Areas.Models.Novels.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/novel")]
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
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _novelModel.GetListNovel(null));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NovelDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetail([FromRoute] long id)
        {
            try
            {
                return Ok(_novelModel.GetNovelAsync(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]NovelCreateUpdateEntity novel)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _novelModel.AddNovel(novel.File, novel);
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
        public async Task<IActionResult> Update([FromRoute] long id, [FromForm] NovelCreateUpdateEntity novel)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _novelModel.UpdateNovel(id, novel, novel.File);
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