﻿using AutoMapper;
using menu.Models.DTO;
using menu.Models;
using core.Model;

namespace menu
{
    public class MappingConfig: Profile
    {
        public MappingConfig() {
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Menu, MenuCreateDto>().ReverseMap();
            
            CreateMap<FoodItem, FoodItemCreateDto>().ReverseMap();
            CreateMap<FoodItem, FoodItemDto>().ReverseMap();
        }
    }
}
