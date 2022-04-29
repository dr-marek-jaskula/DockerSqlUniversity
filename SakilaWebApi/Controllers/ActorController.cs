using Microsoft.AspNetCore.Mvc;
using SakilaWebApi.Services;

namespace SakilaWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ActorController : ControllerBase
{
    private readonly IActorService _actorService;

    public ActorController(IActorService actorService)
    {
        _actorService = actorService;
    }

    [HttpGet]
    public ActionResult<List<ActorDto>> Get()
    {
        var actors = _actorService.GetActors();
        return Ok(actors);
    }

    [HttpGet("example")]
    public ActionResult<List<ActorDto>> GetExample()
    {
        return Ok("Example");
    }
}