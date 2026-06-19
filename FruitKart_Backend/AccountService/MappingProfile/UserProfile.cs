using AccountService.Models.DTO;
using AccountService.Models;
using AutoMapper;

namespace AccountService.MappingProfile
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<NewUserDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) //UserName in DB = Email from DTO
                .ReverseMap();

            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())  
                .ForMember(dest => dest.Email, opt => opt.Ignore())    
                .ReverseMap();

            CreateMap<LoginDTO, User>().ReverseMap();
        }
    }
}