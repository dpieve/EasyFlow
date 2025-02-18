using AutoMapper;

namespace EasyFlow.Application.Common;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Domain.Entities.GeneralSettings, Domain.Entities.GeneralSettings>();
    }
}