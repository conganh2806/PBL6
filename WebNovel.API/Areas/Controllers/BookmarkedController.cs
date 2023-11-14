using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Bookmarked;
using WebNovel.API.Areas.Models.Bookmarked.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/bookmarkeds")]
    [ApiController]
    public class BookMarkedController : BaseController
    {
        private readonly IBookmarkedModel _bookMarkedModel;
        private readonly IServiceProvider _provider;
        public BookMarkedController(IBookmarkedModel bookMarkedModel, IServiceProvider provider) : base(provider)
        {
            _bookMarkedModel = bookMarkedModel;
        }

        [HttpGet("AccountId={AccountId}")]
        [ProducesResponseType(typeof(BookmarkedDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetailByAccount([FromRoute] string AccountId)
        {
            try
            {
                return Ok(await _bookMarkedModel.GetBookmarkedByAccount(AccountId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] BookmarkedCreateUpdateEntity BookMarked)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _bookMarkedModel.AddBookmarked(BookMarked);
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
        public async Task<IActionResult> Update([FromBody] BookmarkedCreateUpdateEntity BookMarked)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _bookMarkedModel.UpdateBookmarked(BookMarked);
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
        public async Task<IActionResult> Delete([FromBody] BookmarkedDeleteEntity bookmarked)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _bookMarkedModel.RemoveBookmarked(bookmarked);
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