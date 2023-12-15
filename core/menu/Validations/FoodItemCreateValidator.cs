using FluentValidation;
using menu.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace menu.Validations
{
    public class FoodItemCreateValidator : AbstractValidator<FoodItemCreateDTO>
    {
        public FoodItemCreateValidator()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Price).GreaterThanOrEqualTo(0);
            RuleFor(model => model.CountLimit).GreaterThan(0);
        }
    }
}
