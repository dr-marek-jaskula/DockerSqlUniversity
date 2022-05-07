using AutoMapper;
using DatabaseModels;
using SakilaWebApi.Models;

namespace SakilaWebApi.Services;

public interface IActorService
{
    List<ActorDto> GetActors();
}

public class ActorService : IActorService
{
    private readonly SakilaDbContext _sakilaDbContext;
    private readonly IMapper _autoMapper;

    public ActorService(SakilaDbContext sakilaDbContext, IMapper autoMapper)
    {
        _sakilaDbContext = sakilaDbContext;
        _autoMapper = autoMapper;
    }

    public List<ActorDto> GetActors()
    {
        var actors = _sakilaDbContext.Actors.ToList();
        var result = _autoMapper.Map<List<ActorDto>>(actors);
        return result;
    }
}