using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Xunit;

namespace TestWebApi;

public class ActorControllerTests
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ActorControllerTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                });
            });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async void TestGetExample()
    {
        //Act

        var response = await _client.GetAsync("/actor/example");

        //Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async void TestGetActor()
    {
        //Act

        var response = await _client.GetAsync("/actor");

        //Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}