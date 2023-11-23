using core;
using Microsoft.AspNetCore.Mvc;
using order.DTO;
using order.Exceptions;
using order.Model;
using order.Repository;
using order.Service;

namespace order.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly IUserRepository _userRepository;

    public OrderController(OrderService orderService, IUserRepository userRepository)
    {
        _orderService = orderService;
        _userRepository = userRepository;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<Order>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult GetOrders(string userId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = _userRepository.GetUser(userGuid);
            
            var orders = _orderService.GetOrders(user);
            
            return Ok(new ApiResponse<IEnumerable<Order>> { Data = orders });
        }
        catch (DataNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpGet("{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Order>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult GetOrder(string userId, string orderId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = _userRepository.GetUser(userGuid);
            
            var orderGuid = Guid.Parse(orderId);
            var order = _orderService.GetOrder(user, orderGuid);
            
            return Ok(new ApiResponse<Order> { Data = order });
        }
        catch (DataNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPost("create/{userId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<Order>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult CreateOrder(string userId, [FromBody] OrderDTO? order)
    {
        if (order == null)
            return BadRequest(ApiResponse.BadRequest("Invalid order data"));

        try
        {
            var userGuid = Guid.Parse(userId);
            var user = _userRepository.GetUser(userGuid);

            var newOrder = _orderService.CreateOrder(user, order);

            return CreatedAtAction(nameof(GetOrder), new { userId = userId, orderId = newOrder.Id }, new ApiResponse<Order> { Data = newOrder });
        }
        catch (DataNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPost("confirm/{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult ConfirmOrder(string userId, string orderId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = _userRepository.GetUser(userGuid);
            
            _orderService.ConfirmOrder(user, Guid.Parse(orderId));
            
            return NoContent();
        }
        catch (DataNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPost("delete/{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public IActionResult DeleteOrder(string userId, string orderId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = _userRepository.GetUser(userGuid);
            
            _orderService.DeleteOrder(user, Guid.Parse(orderId));
            
            return NoContent();
        }
        catch (DataNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }
}
