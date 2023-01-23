using AutoMapper;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace CVManager;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<CVManager.DAL.Models.Degree, DTO.DegreeDto>();
        CreateMap<DTO.DegreeDto, CVManager.DAL.Models.Degree>();
        CreateMap<CVManager.DAL.Models.Cv, DTO.CvCreateUpdateDto>();
        CreateMap<CVManager.DAL.Models.Cv, DTO.CvDetailWithDegreesDto>();

        CreateMap<DTO.CvCreateUpdateDto, CVManager.DAL.Models.Cv>()
        .ForMember
        (dest => dest.DegreeId,
            opt =>
            {
                opt.PreCondition(src => src.DegreeId != 0);
                opt.MapFrom(src => src.DegreeId);
                
        });

        CreateMap<CVManager.DAL.Models.Cv, DTO.CvDetailDto>();
    }
}

// public bool File { get; set; } = false;
//
// public IEnumerable<DegreeDto>? Degrees { get; set; }
// public IFormFile? File { get; set; }
