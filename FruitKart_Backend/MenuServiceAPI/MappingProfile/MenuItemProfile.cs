using AutoMapper;
using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;

namespace MenuServiceAPI.MappingProfile
{
    public class MenuItemProfile : Profile          
    {
        public MenuItemProfile()                   
        {
            CreateMap<MenuItem, MenuItemDTO>();

            CreateMap<MenuItemCreateDTO, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore()); 

            CreateMap<MenuItemUpdateDTO, MenuItem>()
                .ForMember(dest => dest.Image, opt => opt.Ignore()); 
        }
    }
}