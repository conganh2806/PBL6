using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Accounts.Schemas;
using WebNovel.API.Controllers;
using WebNovel.API.Core.Schemas;
using WebNovel.API.Core.Services;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/master/s3")]
    [ApiController]
    public class AwsS3Controller : BaseController
    {
        private readonly IServiceProvider _provider;
        private readonly IAwsS3Service _aws3Services;
        public AwsS3Controller(IServiceProvider provider, IAwsS3Service aws3Services) : base(provider)
        {
            _aws3Services = aws3Services;
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFileFromS3(string folder, string fileName)
        {
            try
            {
                //Có thể bỏ command bên dưới để test trên swagger
                //fileName = "test/share.pdf";
                if (string.IsNullOrEmpty(fileName))
                    return StatusCode(404, "The 'fileName' parameter is required");

                var file = await _aws3Services.GetFile(folder, fileName);

                return File(file, "application/pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(404, ex.Message);
            }
        }
    }
}