using AutoMapper;
using DatabaseModels.Models;
using SakilaWebApi.Models;

namespace SakilaWebApi;

public class ActorMappingProfile : Profile
{
    public ActorMappingProfile()
    {
        CreateMap<Actor, ActorDto>();
    }
}