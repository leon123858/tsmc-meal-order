using FluentValidation;
using menu.Models.DTO;

namespace menu.Validations
{
    public class MenuUpdateValidation: AbstractValidator<MenuUpdateDto>
    {
        public MenuUpdateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Id).GreaterThan(0);
        }
    }
}
