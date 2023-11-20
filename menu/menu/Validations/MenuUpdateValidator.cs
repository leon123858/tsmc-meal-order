using FluentValidation;
using menu.Models.DTO;

namespace menu.Validations
{
    public class MenuUpdateValidator: AbstractValidator<MenuUpdateDto>
    {
        public MenuUpdateValidator()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleForEach(x => x.FoodItems).SetValidator(new FoodItemCreateValidator());
        }
    }
}
