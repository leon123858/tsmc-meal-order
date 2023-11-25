﻿using FluentValidation;
using menu.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace menu.Validations
{
    public class FoodItemCreateValidator : AbstractValidator<FoodItemCreateDto>
    {
        public FoodItemCreateValidator()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleFor(model => model.Price).GreaterThanOrEqualTo(0);
            RuleFor(model => model.Count).GreaterThan(0);
            RuleFor(model => model.Tags.Count).LessThanOrEqualTo(4);
        }
    }
}