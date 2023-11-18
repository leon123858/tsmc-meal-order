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
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<Order>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<Order>))]
    public IActionResult GetOrder(string id)
    {
        try
        {
            var order = _orderService.GetOrder(Guid.Parse(id));
            return Ok(new Response<Order> { Data = order });
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(RequestResponse.NotFound<Order>());
        }
        catch (Exception e)
        {
            return BadRequest(RequestResponse.BadRequest<Order>());
        }
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<Order>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<Order>))]
    public IActionResult CreateOrder([FromBody] Order? order)
    {
        if (order == null)
            return BadRequest(RequestResponse.BadRequest<Order>("Invalid order data"));

        _orderService.CreateOrder(order);

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new Response<Order> { Data = order });
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<Order>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<Order>))]
    public IActionResult UpdateOrder(string id, [FromBody] Order? updatedOrder)
    {
        if (updatedOrder == null)
            return BadRequest(RequestResponse.BadRequest<Order>("Invalid order data"));

        try
        {
            _orderService.UpdateOrder(Guid.Parse(id), updatedOrder);
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(RequestResponse.NotFound<Order>());
        }
        catch (Exception e)
        {
            return BadRequest(RequestResponse.BadRequest<Order>());
        }
    }

    [HttpPut("confirm/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<Order>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<Order>))]
    public IActionResult ConfirmOrder(string id)
    {
        try
        {
            _orderService.ConfirmOrder(Guid.Parse(id));
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(RequestResponse.NotFound<Order>());
        }
        catch (Exception e)
        {
            return BadRequest(RequestResponse.BadRequest<Order>());
        }
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<Order>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response<Order>))]
    public IActionResult DeleteOrder(string id)
    {
        try
        {
            _orderService.DeleteOrder(Guid.Parse(id));
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound(RequestResponse.NotFound<Order>());
        }
        catch (Exception e)
        {
            return BadRequest(RequestResponse.BadRequest<Order>());
        }
    }
}
