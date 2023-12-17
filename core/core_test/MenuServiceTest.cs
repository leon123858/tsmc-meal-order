using core.Model;
using NSubstitute;
using menu.Repository;
using menu.Services;
using NSubstitute.ReturnsExtensions;
using menu.Exceptions;
using menu.Models;

namespace core_test;

[TestFixture]
public class MenuServiceTest
{
    [SetUp]
    public void SetUp()
    {
        _menuRepository = Substitute.For<IMenuRepository>();
        _menuService = new MenuService(_menuRepository);
    }

    private IMenuRepository _menuRepository;
    private MenuService _menuService;

    [Test]
    public void GetMenu_NotFound_ThrowMenuNotFoundException()
    {
        // Arrange
        bool isTempMenu = true;
        string menuId = "non_existent_id";
        _menuRepository.FindAsnyc(menuId, isTempMenu).ReturnsNull();

        // Act
        MenuNotFoundException ex = Assert.ThrowsAsync<MenuNotFoundException>(async () => await _menuService.GetMenuAsync(menuId, isTempMenu));

        // Assert
        Assert.That(ex.Message, Is.EqualTo("Menu does not exist."));
    }

    [Test]
    public void GetFoodItem_NotFound_ThrowFoodItemNotFoundException()
    {
        // Arrange
        Menu menu = new Menu()
        {
            FoodItems = new List<FoodItem>()
        };

        // Act
        FoodItemNotFoundException ex = Assert.Throws<FoodItemNotFoundException>(() => _menuService.GetFoodItem(menu, menu.FoodItems.Count+1));

        // Assert
        Assert.That(ex.Message, Is.EqualTo("Food item does not exist."));
    }
}