using core.Model;
using FluentValidation;
using menu.Models.DTO;

namespace menu.Validations
{
    public class FoodItemValidator : AbstractValidator<FoodItem>
    {
        public FoodItemValidator()
        {
            RuleFor(model => model.Name).NotEmpty()
                .WithMessage("The name of the food item should not be empty.");

            RuleFor(model => model.Price).GreaterThanOrEqualTo(0)
                .WithMessage("The price should not be a negative number.");

            RuleFor(model => model.CountLimit).GreaterThan(0)
                .WithMessage("The limit of count should be greater than 0.");

            RuleFor(model => model.Count).GreaterThanOrEqualTo(0)
                .WithMessage("The current count of the food item should not be a negative number.");

            RuleFor(model => model.Count).LessThanOrEqualTo(model => model.CountLimit)
                .WithMessage("The current count of the food item should not exceed the limit.");
        }
    }
}
