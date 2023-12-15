using core.Model;
using order.Model;

namespace core_test;

[TestFixture]
public class OrderedFoodItemTests
{
    [Test]
    public void Constructor_ValidArguments_InitializesProperties()
    {
        // Arrange
        var foodItem = new FoodItem { Name = "Test Food", Count = 10 };
        const int index = 1;
        const int count = 3;
        const string description = "Test Description";

        // Act
        var orderedFoodItem = new OrderedFoodItem(foodItem, index, count, description);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(orderedFoodItem.Snapshot, Is.EqualTo(foodItem));
            Assert.That(orderedFoodItem.Index, Is.EqualTo(index));
            Assert.That(orderedFoodItem.Count, Is.EqualTo(count));
        });
        Assert.That(orderedFoodItem.Description, Is.EqualTo(description));
    }

    [Test]
    public void Constructor_InvalidIndex_ThrowsArgumentException()
    {
        // Arrange
        var foodItem = new FoodItem { Name = "Test Food", Count = 10 };
        const int index = -1;
        const int count = 3;
        const string description = "Test Description";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new OrderedFoodItem(foodItem, index, count, description));
    }

    [Test]
    public void Constructor_InvalidCount_ThrowsArgumentException()
    {
        // Arrange
        var foodItem = new FoodItem { Name = "Test Food", Count = 10 };
        const int index = 1;
        const int count = 0;
        const string description = "Test Description";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new OrderedFoodItem(foodItem, index, count, description));
    }

    [Test]
    public void Constructor_NullSnapshot_ThrowsArgumentNullException()
    {
        // Arrange
        FoodItem foodItem = null;
        const int index = 1;
        const int count = 3;
        const string description = "Test Description";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OrderedFoodItem(foodItem, index, count, description));
    }
}