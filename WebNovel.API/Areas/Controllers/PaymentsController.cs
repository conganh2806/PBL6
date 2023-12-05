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
    }
}
