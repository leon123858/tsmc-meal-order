using Microsoft.AspNetCore.Mvc;
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

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<IEnumerable<Order>>))]
    public IActionResult GetOrders()
    {
        var orders = _orderService.GetOrders();
        return Ok(new Response<IEnumerable<Order>> { Data = orders });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<Order>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    public IActionResult GetOrder(string id)
    {
        try
        {
            var order = _orderService.GetOrder(Guid.Parse(id));
            return Ok(new Response<Order> { Data = order });
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<Order>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    public IActionResult CreateOrder([FromBody] Order? order)
    {
        if (order == null)
            return BadRequest(ApiResponse.BadRequest("Invalid order data"));

        _orderService.CreateOrder(order);

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new Response<Order> { Data = order });
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    public IActionResult UpdateOrder(string id, [FromBody] Order? updatedOrder)
    {
        if (updatedOrder == null)
            return BadRequest(ApiResponse.BadRequest("Invalid order data"));

        try
        {
            _orderService.UpdateOrder(Guid.Parse(id), updatedOrder);
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpPut("confirm/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    public IActionResult ConfirmOrder(string id)
    {
        try
        {
            _orderService.ConfirmOrder(Guid.Parse(id));
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<object>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<object>))]
    public IActionResult DeleteOrder(string id)
    {
        try
        {
            _orderService.DeleteOrder(Guid.Parse(id));
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(ApiResponse.NotFound());
        }
        catch (Exception e)
        {
            return BadRequest(ApiResponse.BadRequest());
        }
    }
}
