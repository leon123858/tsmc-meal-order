﻿using FluentValidation;
using menu.Models.DTO;

namespace menu.Validations
{
    public class MenuCreateValidator: AbstractValidator<MenuCreateDto>
    {
        public MenuCreateValidator()
        {
            RuleFor(model => model.Name).NotEmpty();
            RuleForEach(x => x.FoodItems).SetValidator(new FoodItemCreateValidator());
        }
    }
}