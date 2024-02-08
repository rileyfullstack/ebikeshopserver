using System;
using ebikeshopserver.Authorization;
using ebikeshopserver.Exceptions;
using ebikeshopserver.Models.Order;
using ebikeshopserver.Models.SellPosts;
using ebikeshopserver.Models.User;
using ebikeshopserver.Services;
using ebikeshopserver.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ebikeshopserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
	{
        private OrdersService _ordersService;

        public OrderController(IMongoClient mongoClient)
		{
            _ordersService = new OrdersService(mongoClient);
		}

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserOrders([FromQuery] string userId) //Only for users where the token = the user Id and admins.
        {
            AuthorizationHelper.AuthorizeUserOrAdmin(HttpContext, userId);

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
        [Authorize]
        public async Task<IActionResult> GetOrderById(string orderId) //Check that the requested Id was by the user who created it or an admin
        {
            try
            {
                Order order = await _ordersService.GetOrderByIdAsync(orderId);
                AuthorizationHelper.AuthorizeUserOrAdmin(HttpContext, order.UserId);
                return Ok(order);
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            try
            {
                string newOrderId = await _ordersService.CreateNewOrderAsync(order);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = newOrderId }, order);
            } catch (NoPostsFoundException ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("ByDateRange")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var userId = UserIdProvider.GetUserId(HttpContext);
            try
            {
                var orders = await _ordersService.GetOrdersByDateRangeAsync(start, end, userId);
                if (orders == null || orders.Count == 0)
                {
                    return NotFound("No orders found within the specified date range.");
                }
                return Ok(orders);
            }
            catch(NoPostsFoundException ex)
            {
                return NotFound("No orders at this date range have been found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}

