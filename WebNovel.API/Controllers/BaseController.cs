using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Core.Services;

namespace WebNovel.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ProducesResponseType((int)HttpStatusCode.NotAcceptable)]
    public class BaseController : ControllerBase
    {
        protected readonly ILogService _logService;
        protected readonly ILogger<BaseController> _logger;

        public BaseController()
        {
        }

        public BaseController(
            IServiceProvider provider
        )
        {
            ILogService logService = provider.GetService<ILogService>();
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }
    }
}