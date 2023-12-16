using FluentValidation;
using menu.Models.DTO;

namespace menu.Validations
{
    public class MenuCreateValidator: AbstractValidator<MenuCreateDTO>
    {
        public MenuCreateValidator()
        {
            RuleFor(model => model.Name).NotEmpty()
                .WithMessage("The name of the menu should not be empty.");

            RuleForEach(x => x.FoodItems).SetValidator(new FoodItemCreateValidator());
        }
    }
}
