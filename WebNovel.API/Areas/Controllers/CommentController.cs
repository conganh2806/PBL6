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
        public async Task<IActionResult> Create([FromBody] CommentCreateEntity Comment)
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

        [HttpPut]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] CommentUpdateEntity Comment)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _commentModel.UpdateComment(Comment);
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
        public async Task<IActionResult> Delete([FromBody] CommentDeleteEntity comment)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _commentModel.RemoveComment(comment);
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