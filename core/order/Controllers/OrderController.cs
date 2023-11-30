using core;
using Microsoft.AspNetCore.Mvc;
using order.DTO.Web;
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
    private readonly ILogger<OrderController> _logger;

    public OrderController(OrderService orderService, IUserRepository userRepository, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<IEnumerable<OrderWebDTO>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetOrders(string userId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUser(userGuid);

            var orders = await _orderService.GetOrders(user);
            var orderDtos = orders.Select(_ => (OrderWebDTO)_);

            return Ok(new ApiResponse<IEnumerable<OrderWebDTO>> { Data = orderDtos });
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (FormatException e)
        {
            _logger.LogError("Invalid userId, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest("Invalid userId"));
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpGet("{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Order>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetOrder(string userId, string orderId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUser(userGuid);

            var orderGuid = Guid.Parse(orderId);
            var order = await _orderService.GetOrder(user, orderGuid);

            return Ok(new ApiResponse<Order> { Data = order });
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (FormatException e)
        {
            _logger.LogError("Invalid Id, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest("Invalid Id"));
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPost("create/{userId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<Order>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateOrder(string userId, [FromBody] CreateOrderWebDTO? order)
    {
        if (order == null)
            return BadRequest(ApiResponse.BadRequest("Invalid order data"));

        try
        {
            var userGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUser(userGuid);

            var newOrder = await _orderService.CreateOrder(user, order);

            return CreatedAtAction(nameof(GetOrder), new { userId, orderId = newOrder.Id },
                new ApiResponse<Order> { Data = newOrder });
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (FormatException e)
        {
            _logger.LogError("Invalid userId, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest("Invalid userId"));
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPost("confirm/{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> ConfirmOrder(string userId, string orderId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUser(userGuid);

            await _orderService.ConfirmOrder(user, Guid.Parse(orderId));

            return NoContent();
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (FormatException e)
        {
            _logger.LogError("Invalid Id, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest("Invalid Id"));
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPost("delete/{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> DeleteOrder(string userId, string orderId)
    {
        try
        {
            var userGuid = Guid.Parse(userId);
            var user = await _userRepository.GetUser(userGuid);

            await _orderService.DeleteOrder(user, Guid.Parse(orderId));

            return NoContent();
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (FormatException e)
        {
            _logger.LogError("Invalid Id, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest("Invalid Id"));
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest());
        }
    }
}