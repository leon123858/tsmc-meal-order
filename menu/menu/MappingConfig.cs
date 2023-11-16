using AutoMapper;
using menu.Models.DTO;
using menu.Models;

namespace menu
{
    public class MappingConfig: Profile
    {
        public MappingConfig() {
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Menu, MenuCreateDto>().ReverseMap();
            CreateMap<Menu, MenuUpdateDto>().ReverseMap();
            
            CreateMap<FoodItem, FoodItemCreateDto>().ReverseMap();
            CreateMap<FoodItem, FoodItemDto>().ReverseMap();
        }
    }
}
