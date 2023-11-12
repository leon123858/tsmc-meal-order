using AutoMapper;
using menu.Models.DTO;
using menu.Models;

namespace menu
{
    public class MappingConfig: Profile
    {
        public MappingConfig() { 
            CreateMap<Menu, MenuCreateDto>().ReverseMap();
            CreateMap<Menu, MenuDto>().ReverseMap();
        }
    }
}
