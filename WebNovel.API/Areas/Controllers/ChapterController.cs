using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebNovel.API.Areas.Models.Chapter;
using WebNovel.API.Areas.Models.Chapter.Schemas;
using WebNovel.API.Commons.Enums;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using WebNovel.API.Databases.Entities;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/chapter")]
    public class ChapterController : BaseController
    {
        private readonly IChapterModel _chapterModel;
        private readonly IServiceProvider _provider;
        public ChapterController(IChapterModel chapterModel, IServiceProvider provider) : base(provider)
        {
            _chapterModel = chapterModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ChapterDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search() {
            try
            {
                return Ok(await _chapterModel.GetListChapter());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] ChapterCreateUpdateEntity chapter) {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _chapterModel.AddChapter(chapter);
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
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] ChapterCreateUpdateEntity chapter)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _chapterModel.UpdateChapter(id, chapter);
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