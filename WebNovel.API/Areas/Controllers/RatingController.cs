using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Rating;
using WebNovel.API.Areas.Models.Rating.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/ratings")]
    [ApiController]
    public class RatingController : BaseController
    {
        private readonly IRatingModel _ratingModel;
        private readonly IServiceProvider _provider;
        public RatingController(IRatingModel ratingModel, IServiceProvider provider) : base(provider)
        {
            _ratingModel = ratingModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(RatingDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _ratingModel.GetListRating());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("NovelId={NovelId}")]
        [ProducesResponseType(typeof(RatingDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetailByNovel([FromRoute] string NovelId)
        {
            try
            {
                return Ok(_ratingModel.GetRatingByNovel(NovelId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("AccountId={AccountId}")]
        [ProducesResponseType(typeof(RatingDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetailByAccount([FromRoute] string AccountId)
        {
            try
            {
                return Ok(_ratingModel.GetRatingByAccount(AccountId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{AccountId}/{NovelId}")]
        [ProducesResponseType(typeof(RatingDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetail([FromRoute] string AccountId, [FromRoute] string NovelId)
        {
            try
            {
                return Ok(_ratingModel.GetRating(AccountId, NovelId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] RatingCreateUpdateEntity rating)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _ratingModel.AddRating(rating);
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

        [HttpPut("{AccountId}/{NovelId}")]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromRoute] string AccountId, [FromRoute] string NovelId, [FromBody] RatingCreateUpdateEntity rating)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _ratingModel.UpdateRating(AccountId, NovelId, rating);
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