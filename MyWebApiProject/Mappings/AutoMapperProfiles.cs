using AutoMapper;
using MyWebApiProject.Models.Domain;
using MyWebApiProject.Models.DTO;

namespace MyWebApiProject.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<AddRegionRequestDto, Region>().ReverseMap();
            CreateMap<UpdateRegionRequestDto, Region>().ReverseMap();
        }

    }
}
