using System;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.Order;
using ebikeshopserver.Models.SellPosts;
using ebikeshopserver.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ebikeshopserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
	{
        private OrdersService _ordersService;

        public OrderController(IMongoClient mongoClient)
		{
            _ordersService = new OrdersService(mongoClient);
		}

        [HttpGet]
        public async Task<IActionResult> GetUserOrders([FromQuery] string userId) //Only for users where the token = the user Id and admins.
        {
            try
            {
                List<Order> result = await _ordersService.GetOrdersByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex) when (ex is UserNotFoundException || ex is OrderNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(string orderId) //Check that the requested Id was by the user who created it or an admin
        {
            try
            {
                Order order = await _ordersService.GetOrderByIdAsync(orderId);
                return Ok(order);
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            try
            {
                string newOrderId = await _ordersService.CreateNewOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = newOrderId }, order);
            } catch (NoPostsFoundException ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("ByDateRange")]
        public async Task<IActionResult> GetOrdersByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var orders = await _ordersService.GetOrdersByDateRangeAsync(start, end);
                if (orders == null || orders.Count == 0)
                {
                    return NotFound("No orders found within the specified date range.");
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Consider logging the exception details
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}

