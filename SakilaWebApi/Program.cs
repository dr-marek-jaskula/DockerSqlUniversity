using DatabaseModels;
using Microsoft.EntityFrameworkCore;
using SakilaWebApi.Middlewares;
using SakilaWebApi.Services;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SakilaWebApi.Tests")]
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console() //to console, because at the very beginning we have only console
            .CreateBootstrapLogger();

try
{
    Log.Information("Staring the web host");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    //Add connection string file for locally used application
    //builder.Configuration.AddJsonFile(@"C:\Users\Marek\source\repos\DockerSqlUniversity\ConnectionStrings.json");
    //Add docker secrets that are placed in the "/run/secrets" folder (remember to add ConnectionStrings.json to docker ignore)
    builder.Configuration.AddJsonFile("/run/secrets/db_ConnectionString");

    builder.Services.AddControllers();
    builder.Services.AddDbContext<SakilaDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("FromContainerToContainer")));

    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

    builder.Services.AddScoped<IActorService, ActorService>();

    builder.Services.AddScoped<ErrorHandlingMiddleware>();

    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    app.UseMiddleware<ErrorHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;