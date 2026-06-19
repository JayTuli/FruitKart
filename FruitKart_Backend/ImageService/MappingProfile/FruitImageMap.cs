using AutoMapper;
using ImageService.Models.DTO;
using ImageService.Models;

namespace ImageService.MappingProfile
{
    public class FruitImageMap : Profile
    {
        public FruitImageMap()
        {
            CreateMap<Images, UpdatedImagesDTO>().ReverseMap();
            CreateMap<NewImagesDTO, Images>().ReverseMap();
        }
    }
}