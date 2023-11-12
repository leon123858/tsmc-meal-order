using FluentValidation;
using menu.Models.DTO;

namespace menu.Validations
{
    public class MenuCreateValidation: AbstractValidator<MenuCreateDto>
    {
        public MenuCreateValidation()
        {
            RuleFor(model => model.Name).NotEmpty();
        }
    }
}
