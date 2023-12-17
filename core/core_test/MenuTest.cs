using FluentValidation.TestHelper;
using menu.Validations;
using menu.Models.DTO;

namespace core_test;

[TestFixture]
public class MenuTest
{
    private MenuCreateValidator _menuCreateValidator;

    [SetUp]
    public void SetUp()
    {
        _menuCreateValidator = new MenuCreateValidator();
    }

    [Test]
    public void Menu_EmptyName_ShouldHaveError()
    {
        // Arrange
        var menu = new MenuCreateDTO { Name = "" };
        var errorMsg = "The name of the menu should not be empty.";

        // Act
        var result = _menuCreateValidator.TestValidate(menu);

        // Assert
        result.ShouldHaveValidationErrorFor(m => m.Name).WithErrorMessage(errorMsg);
    }
}