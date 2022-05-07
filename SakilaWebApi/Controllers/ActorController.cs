using Microsoft.AspNetCore.Mvc;
using SakilaWebApi.Services;
using SakilaWebApi.Models;
using Serilog;

namespace SakilaWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ActorController : ControllerBase
{
    private readonly IActorService _actorService;
    private readonly ILogger<ActorController> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public ActorController(IActorService actorService, ILogger<ActorController> logger, IDiagnosticContext diagnosticContext)
    {
        _actorService = actorService;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    [HttpGet]
    public ActionResult<List<ActorDto>> Get()
    {
        var actors = _actorService.GetActors();
        return Ok(actors);
    }

    [HttpGet("example")]
    public ActionResult<string> GetExample()
    {
        return Ok("Example");
    }

    [HttpGet("exception")]
    public ActionResult GetException()
    {
        _logger.LogInformation("Inside of PingException");
        throw new InvalidOperationException("Something bad happened");
    }

    //IDiagnosticContext is what serilog uses
    [HttpGet("context")]
    public ActionResult<string> PingDiagnosticContext()
    {
        _diagnosticContext.Set("UserId", "someone");
        return "Context set to seq";
    }
}