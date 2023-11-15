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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Order>))]
    public IActionResult GetOrders()
    {
        var orders = _orderService.GetOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public IActionResult GetOrder(int id)
    {
        try
        {
            var order = _orderService.GetOrder(id);
            return Ok(order);
        }
        catch (OrderNotFoundException e)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest("Unknown Error");
        }
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Order))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public IActionResult CreateOrder([FromBody] Order? order)
    {
        if (order == null)
            return BadRequest("Invalid order data");

        _orderService.CreateOrder(order);

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public IActionResult UpdateOrder(int id, [FromBody] Order? updatedOrder)
    {
        if (updatedOrder == null)
            return BadRequest("Invalid order data");

        try
        {
            _orderService.UpdateOrder(id, updatedOrder);
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }

    [HttpPut("confirm/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public IActionResult ConfirmOrder(int id)
    {
        try
        {
            _orderService.ConfirmOrder(id);
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public IActionResult DeleteOrder(int id)
    {
        try
        {
            _orderService.DeleteOrder(id);
            return NoContent();
        }
        catch (OrderNotFoundException e)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }
}
