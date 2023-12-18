using AutoMapper;
using menu.Models.DTO;
using menu.Models;
using core.Model;

namespace menu
{
    public class MappingConfig: Profile
    {
        public MappingConfig() {
            CreateMap<Menu, MenuDTO>().ReverseMap();
            CreateMap<Menu, MenuCreateDTO>().ReverseMap();
            CreateMap<Menu, RecFoodItemDTO>()
                .ForMember(dto => dto.MenuId, cf => cf.MapFrom(menu => menu.Id));

            CreateMap<FoodItem, FoodItemDTO>().ReverseMap();
            CreateMap<FoodItem, FoodItemCreateDTO>();
            CreateMap<FoodItemCreateDTO, FoodItem>()
                .ForMember(item => item.Count, cf => cf.MapFrom(dto => dto.CountLimit));
           
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
