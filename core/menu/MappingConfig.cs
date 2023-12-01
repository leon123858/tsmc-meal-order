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
            
            CreateMap<FoodItem, FoodItemCreateDTO>().ReverseMap();
            CreateMap<FoodItem, FoodItemDTO>().ReverseMap();

            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}
