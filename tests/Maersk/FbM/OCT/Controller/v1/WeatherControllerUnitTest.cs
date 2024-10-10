using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace oct_template.tests.Maersk.FbM.OCT.Controller.v1;

public class WeatherControllerUnitTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherControllerUnitTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    // [Theory]
    // [InlineData("/api/v1/Weather")]
    // [InlineData("/version")]
    // public async Task Get_EndpointsReturnSuccess(string endpoint)
    // {
    //     // Arrange
    //     var client = _factory.CreateClient();
    //
    //     // Act
    //     var response = await client.GetAsync(endpoint);
    //
    //     // Assert
    //     response.EnsureSuccessStatusCode();
    // }
}