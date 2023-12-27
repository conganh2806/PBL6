using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Reports;
using WebNovel.API.Areas.Models.Reports.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportModel _reportModel;
        private readonly IServiceProvider _provider;
        public ReportController(IReportModel reportModel, IServiceProvider provider) : base(provider)
        {
            _reportModel = reportModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ReportDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Search()
        {
            try
            {
                return Ok(await _reportModel.GetListReport());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ReportCreateEntity report)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _reportModel.AddReport(report);
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