using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Comment;
using WebNovel.API.Areas.Models.Comment.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : BaseController
    {
        private readonly ICommentModel _commentModel;
        private readonly IServiceProvider _provider;
        public CommentController(ICommentModel commentModel, IServiceProvider provider) : base(provider)
        {
            _commentModel = commentModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(CommentDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _commentModel.GetListComment());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("NovelId={NovelId}")]
        [ProducesResponseType(typeof(CommentDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetailByNovel([FromRoute] string NovelId)
        {
            try
            {
                return Ok(await _commentModel.GetCommentByNovel(NovelId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("AccountId={AccountId}")]
        [ProducesResponseType(typeof(CommentDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetailByAccount([FromRoute] string AccountId)
        {
            try
            {
                return Ok(await _commentModel.GetCommentByAccount(AccountId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{AccountId}/{NovelId}")]
        [ProducesResponseType(typeof(CommentDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetailByAccountNovel([FromRoute] string AccountId, [FromRoute] string NovelId)
        {
            try
            {
                return Ok(await _commentModel.GetCommentByAccountNovel(AccountId, NovelId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(CommentDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetail([FromRoute] long Id)
        {
            try
            {
                return Ok(await _commentModel.GetComment(Id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CommentCreateUpdateEntity Comment)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _commentModel.AddComment(Comment);
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

        [HttpPut("{Id}")]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] long Id, [FromBody] CommentCreateUpdateEntity Comment)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _commentModel.UpdateComment(Id, Comment);
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