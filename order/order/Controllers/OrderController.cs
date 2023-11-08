using Microsoft.AspNetCore.Mvc;
using order.Model;
using order.Repository;

namespace order.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly OrderRepository _repository;

    public OrderController(OrderRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Order>> GetOrders()
    {
        var orders = _repository.GetOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Order> GetOrder(string id)
    {
        var guid = Guid.Parse(id);

        var order = _repository.GetOrder(guid);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost("create")]
    public ActionResult<Order> CreateOrder([FromBody] Order order)
    {
        _repository.CreateOrder(order);

        return CreatedAtAction("GetOrder", new { id = order.Id }, order);
    }

    [HttpPut("update/{id}")]
    public IActionResult UpdateOrder(string id, [FromBody] Order updatedOrder)
    {
        var guid = Guid.Parse(id);

        _repository.UpdateOrder(guid, updatedOrder);

        return NoContent();
    }

    [HttpPut("confirm/{id}")]
    public IActionResult ConfirmOrder(string id)
    {
        var guid = Guid.Parse(id);

        _repository.ConfirmOrder(guid);

        return NoContent();
    }

    [HttpDelete("delete/{id}")]
    public IActionResult DeleteOrder(string id)
    {
        var guid = Guid.Parse(id);

        _repository.DeleteOrder(guid);

        return NoContent();
    }
}