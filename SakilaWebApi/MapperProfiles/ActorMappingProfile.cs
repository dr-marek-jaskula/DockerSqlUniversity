using AutoMapper;
using DatabaseModels.Models;

namespace SakilaWebApi;

public class ActorMappingProfile : Profile
{
    public ActorMappingProfile()
    {
        CreateMap<Actor, ActorDto>();
    }
}