using DatabaseModels;
using Microsoft.EntityFrameworkCore;
using SakilaWebApi.Services;
using System.Reflection;
using System.Runtime.CompilerServices;

// [assembly: InternalsVisibleTo("SakilaWebApi.Tests")]
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<SakilaDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("FromContainerToContainer")));

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IActorService, ActorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();