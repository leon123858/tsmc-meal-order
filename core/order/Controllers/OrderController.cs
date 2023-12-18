using core;
using Microsoft.AspNetCore.Mvc;
using order.DTO.Web;
using order.Exceptions;
using order.Model;
using order.Repository;
using order.Service;

namespace order.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly OrderService _orderService;
    private readonly IUserRepository _userRepository;

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
            var user = await _userRepository.GetUser(userId);

            var orders = await _orderService.GetOrders(user);
            var orderDtos = orders.Select(_ => (OrderWebDTO)_);

            return Ok(new ApiResponse<IEnumerable<OrderWebDTO>> { Data = orderDtos });
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest(e.Message));
        }
    }

    [HttpGet("{userId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<OrderWebDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetOrder(string userId, string orderId)
    {
        try
        {
            var user = await _userRepository.GetUser(userId);

            var orderGuid = Guid.Parse(orderId);
            var order = await _orderService.GetOrder(user, orderGuid);

            return Ok(new ApiResponse<OrderWebDTO> { Data = (OrderWebDTO)order });
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest(e.Message));
        }
    }

    [HttpPost("create/{userId}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<OrderWebDTO>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> CreateOrder(string userId, [FromBody] CreateOrderWebDTO? order)
    {
        if (order == null)
            return BadRequest(ApiResponse.BadRequest("Invalid order data"));

        try
        {
            var user = _userRepository.GetUser(userId);
            var restaurant = _userRepository.GetUser(order.MenuId);

            var newOrder = await _orderService.CreateOrder(await user, await restaurant, order);

            return CreatedAtAction(nameof(GetOrder), new { userId, orderId = newOrder.Id },
                new ApiResponse<OrderWebDTO> { Data = (OrderWebDTO)newOrder });
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest(e.Message));
        }
    }

    [HttpPost("confirm/{restaurantId}/{orderId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> ConfirmOrder(string restaurantId, string orderId)
    {
        try
        {
            var restaurant = await _userRepository.GetUser(restaurantId);

            await _orderService.ConfirmOrder(restaurant, Guid.Parse(orderId));

            return NoContent();
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest(e.Message));
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
            var user = await _userRepository.GetUser(userId);

            await _orderService.DeleteOrder(user, Guid.Parse(orderId));

            return NoContent();
        }
        catch (DataNotFoundException e)
        {
            _logger.LogError("Data not found, Exception: {Message}", e.Message);
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest(e.Message));
        }
    }

    [HttpPost("notify/{mealTypeString}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> NotifyCustomers(string mealTypeString)
    {
        try
        {
            var mealType = Enum.Parse<MealType>(mealTypeString);

            await _orderService.NotifyCustomers(mealType);

            return Ok(new ApiResponse<string> { Data = "Notification sent" });
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown, Exception: {Message}", e.Message);
            return BadRequest(ApiResponse.BadRequest(e.Message));
        }
    }
}