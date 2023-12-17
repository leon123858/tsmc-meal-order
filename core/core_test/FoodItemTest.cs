using FluentValidation;
using FluentValidation.TestHelper;
using menu.Validations;
using core.Model;
using menu.Models.DTO;

namespace core_test;

[TestFixture]
public class FoodItemTest
{
    private FoodItemValidator _foodItemValidator;
    private FoodItemCreateValidator _foodItemCreateValidator;
    private MenuCreateValidator _menuCreateValidator;

    [SetUp]
    public void SetUp()
    {
        _foodItemValidator = new FoodItemValidator();
        _foodItemCreateValidator = new FoodItemCreateValidator();
        _menuCreateValidator = new MenuCreateValidator();
    }

    [Test]
    public void FoodItem_NegativeCount_ShouldHaveError()
    {
        // Arrange
        var foodItem = new FoodItem { Count = -1 };
        var errorMsg = "The current count of the food item should not be a negative number.";

        // Act
        var result = _foodItemValidator.TestValidate(foodItem);

        // Assert
        result.ShouldHaveValidationErrorFor(m => m.Count).WithErrorMessage(errorMsg);
    }

    [Test]
    public void FoodItem_CountOverLimit_ShouldHaveError()
    {
        // Arrange
        var model = new FoodItem { Count = 11, CountLimit = 10 };
        var errorMsg = "The current count of the food item should not exceed the limit.";

        // Act
        var result = _foodItemValidator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(m => m.Count).WithErrorMessage(errorMsg);
    }

    [Test]
    public void FoodItem_EmptyName_ShouldHaveError()
    {
        // Arrange
        var foodItem = new FoodItem { Name = ""};
        var foodItemCreateDto = new FoodItemCreateDTO { Name = "" };
        var menuCrateDto = new MenuCreateDTO { FoodItems = new() { foodItemCreateDto } };
        var errorMsg = "The name of the food item should not be empty.";

        // Act
        var result = _foodItemValidator.TestValidate(foodItem);
        var dtoResult = _foodItemCreateValidator.TestValidate(foodItemCreateDto);
        var menuResult = _menuCreateValidator.TestValidate(menuCrateDto);

        // Assert
        result.ShouldHaveValidationErrorFor(m => m.Name).WithErrorMessage(errorMsg);
        dtoResult.ShouldHaveValidationErrorFor(m => m.Name).WithErrorMessage(errorMsg);
        menuResult.ShouldHaveValidationErrorFor("FoodItems[0].Name").WithErrorMessage(errorMsg);
    }

    [Test]
    public void FoodItem_CountLimitNotGreaterthanZero_ShouldHaveError()
    {
        // Arrange
        var foodItem = new FoodItem { CountLimit = 0 };
        var foodItemCreateDto = new FoodItemCreateDTO { CountLimit = 0 };
        var menuCrateDto = new MenuCreateDTO { FoodItems = new() { foodItemCreateDto } };
        var errorMsg = "The limit of count should be greater than 0.";

        // Act
        var result = _foodItemValidator.TestValidate(foodItem);
        var dtoResult = _foodItemCreateValidator.TestValidate(foodItemCreateDto);
        var menuResult = _menuCreateValidator.TestValidate(menuCrateDto);

        // Assert
        result.ShouldHaveValidationErrorFor(m => m.CountLimit).WithErrorMessage(errorMsg);
        dtoResult.ShouldHaveValidationErrorFor(m => m.CountLimit).WithErrorMessage(errorMsg);
        menuResult.ShouldHaveValidationErrorFor("FoodItems[0].CountLimit").WithErrorMessage(errorMsg);
    }
}