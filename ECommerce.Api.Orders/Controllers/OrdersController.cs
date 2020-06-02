using ECommerce.Api.Orders.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderProvider _ordersProvider;
        public OrdersController(IOrderProvider ordersProvider)
        {
            _ordersProvider = ordersProvider;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetProductAsync(int customerId)
        {
            var result = await _ordersProvider.GetCustomerOrdersAsync(customerId);

            if (result.IsSuccess)
            {
                return Ok(result.orders);
            }

            return NotFound();
        }
    }
}
