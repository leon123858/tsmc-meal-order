using order.Model;

namespace core_test;

[TestFixture]
public class OrderTests
{
    [Test]
    public void Confirm_WhenOrderIsInInitStatus_ShouldChangeStatusToPreparing()
    {
        // Arrange
        var order = new Order { Status = OrderStatus.Init };

        // Act
        order.Confirm();

        // Assert
        Assert.That(order.Status, Is.EqualTo(OrderStatus.Preparing));
    }

    [Test]
    public void Confirm_WhenOrderIsNotInInitStatus_ShouldThrowException()
    {
        // Arrange
        var order = new Order { Status = OrderStatus.Preparing };

        // Act & Assert
        Assert.Throws<Exception>(() => order.Confirm());
    }

    [Test]
    public void Cancel_WhenOrderIsInInitStatus_ShouldChangeStatusToCanceled()
    {
        // Arrange
        var order = new Order { Status = OrderStatus.Init };

        // Act
        order.Cancel();

        // Assert
        Assert.That(order.Status, Is.EqualTo(OrderStatus.Canceled));
    }

    [Test]
    public void Cancel_WhenOrderIsNotInInitStatus_ShouldThrowException()
    {
        // Arrange
        var order = new Order { Status = OrderStatus.Preparing };

        // Act & Assert
        Assert.Throws<Exception>(() => order.Cancel());
    }
}