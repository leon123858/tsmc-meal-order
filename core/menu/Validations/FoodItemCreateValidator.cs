﻿using core.Model;
using FluentValidation;
using menu.Models.DTO;

namespace menu.Validations
{
    public class FoodItemCreateValidator : AbstractValidator<FoodItemCreateDTO>
    {
        public FoodItemCreateValidator()
        {
            RuleFor(model => model.Name).NotEmpty()
                .WithMessage("The name of the food item should not be empty.");

            RuleFor(model => model.Price).GreaterThanOrEqualTo(0)
                .WithMessage("The price should not be a negative number.");
            
            RuleFor(model => model.CountLimit).GreaterThan(0)
                .WithMessage("The limit of count should be greater than 0.");
        }
    }
}
