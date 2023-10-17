using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Preferences;
using WebNovel.API.Areas.Models.Preferences.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/preferences")]
    [ApiController]
    public class PreferencesController : BaseController
    {
        private readonly IPreferencesModel _preferencesModel;
        private readonly IServiceProvider _provider;
        public PreferencesController(IPreferencesModel preferencesModel, IServiceProvider provider) : base(provider)
        {
            _preferencesModel = preferencesModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PreferencesDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _preferencesModel.GetListPreference());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PreferencesDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDetail([FromRoute] long id)
        {
            try
            {
                return Ok(_preferencesModel.GetPreference(id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create([FromBody] PreferencesCreateUpdateEntity preference)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _preferencesModel.AddPreference(preference);
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
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] PreferencesCreateUpdateEntity preference)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _preferencesModel.UpdatePreference(id, preference);
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