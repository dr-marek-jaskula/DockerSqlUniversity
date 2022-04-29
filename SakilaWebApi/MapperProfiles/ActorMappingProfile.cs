using AutoMapper;

namespace SakilaWebApi;

public class ActorMappingProfile : Profile
{
    public ActorMappingProfile()
    {
        CreateMap<Actor, ActorDto>();
    }
}