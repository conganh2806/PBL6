using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Payments;
using WebNovel.API.Areas.Models.Payments.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using WebNovel.API.Core.Extentions;
using WebNovel.API.Core.Services.VnPay.Schemas;
using static WebNovel.API.Commons.Enums.CodeResonse;

namespace WebNovel.API.Areas.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentsController : BaseController
    {
        private readonly IPaymentModel _paymentModel;
        private readonly IServiceProvider _provider;
        public PaymentsController(IPaymentModel paymentModel,
        IServiceProvider provider) : base(provider)
        {
            _paymentModel = paymentModel;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaymentHistoryDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetail()
        {
            try
            {
                return Ok(await _paymentModel.GetAllPaymentHistory());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("AccountId={accountId}")]
        [ProducesResponseType(typeof(PaymentHistoryDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetailByAccount([FromRoute] string accountId)
        {
            try
            {
                return Ok(await _paymentModel.GetPaymentHistory(accountId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreatePaymentEntity payment)
        {
            try
            {
                return Ok(await _paymentModel.CreatePayment(payment));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet]
        [Route("vnpay-return")]
        public async Task<IActionResult> VnpayReturn([FromQuery] VnpayPayResponse response)
        {
            try
            {
                var resultWithData = await _paymentModel.ProcessVnpayPaymentReturn(response);
                var returnModel = resultWithData.Data.Item1 as PaymentReturnDto;
                string returnUrl = resultWithData.Data.Item2 as string;

                if (returnUrl.EndsWith("/"))
                    returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
                return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }

        }

        [HttpPost("request-payout")]
        [Authorize]
        public async Task<IActionResult> CreateRequestPayout([FromBody] CreatePayoutDto request)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _paymentModel.CreateRequestPayout(request);
                }
                else
                {
                    response.Code = CodeResponse.NOT_VALIDATE;
                }
                return StatusCode(response.Code, response);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPut("accept-request-payout/{PayoutId}")]
        [Authorize]
        public async Task<IActionResult> AcceptRequestPayout(long PayoutId)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _paymentModel.Payout(PayoutId);
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

        [HttpDelete("cancel-request-payout")]
        [Authorize]
        public async Task<IActionResult> DeleteRequestPayout([FromBody] DeletePayoutDto request)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _paymentModel.DeleteRequestPayout(request);
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

        [HttpGet("get-payout")]
        [Authorize]
        public async Task<IActionResult> GetPayout()
        {
            try
            {
                return Ok(await _paymentModel.GetAllPayout());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("get-payout/{AccountId}")]
        [Authorize]
        public async Task<IActionResult> GetPayout(string AccountId)
        {
            try
            {
                return Ok(await _paymentModel.GetAllPayout(AccountId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("admin-revenue")]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetRevenue()
        {
            try
            {
                return Ok(await _paymentModel.GetAdminRevenue());
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpGet("creator-revenue/{AccountId}")]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetRevenue(string AccountId)
        {
            try
            {
                return Ok(await _paymentModel.GetRevenue(AccountId));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }
    }
}
