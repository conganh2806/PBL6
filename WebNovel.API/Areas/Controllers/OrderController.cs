using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebNovel.API.Areas.Models.Orders;
using WebNovel.API.Areas.Models.Orders.Schemas;
using WebNovel.API.Commons.Schemas;
using WebNovel.API.Controllers;
using static WebNovel.API.Commons.Enums.CodeResonse;
namespace WebNovel.API.Areas.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderModel _orderModel;
        private readonly IServiceProvider _provider;
        public OrderController(IOrderModel orderModel, IServiceProvider provider) : base(provider)
        {
            _orderModel = orderModel;
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> GetDetail([FromRoute] string Id)
        {
            try
            {
                return Ok(await _orderModel.GetOrder(Id));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Error = e.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseInfo), (int)HttpStatusCode.OK)]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] OrderCreateEntity order)
        {
            try
            {
                ResponseInfo response = new ResponseInfo();
                if (ModelState.IsValid)
                {
                    response = await _orderModel.AddOrder(order);
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
