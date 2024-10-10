using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Maersk.FbM.OCT.Contracts;
using Maersk.FbM.OCT.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Maersk.FbM.OCT.Controller.Extensions;

public class HttpContextExtensionsTests
{
    private const string HeaderName = "X-MAERSK-RID";
    
    [Fact]
    public void GetClientName_ShouldReturnClientName_WhenClientInfoExistsInQuery()
    {
        // Arrange
        const string clientName = "TestClient";
        var context = new DefaultHttpContext();
        var clientInfo = new ClientInfo { Name = clientName };
        var clientInfoJson = JsonSerializer.Serialize(clientInfo);
        context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>
        {
            ["Client"] = new(clientInfoJson)
        });

        // Act
        var result = context.GetClientName();

        // Assert
        result.Should().Be(clientName);
    }

    [Fact]
    public void GetClientName_ShouldReturnEmptyString_WhenClientInfoDoesNotExistInQuery()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Query = new QueryCollection(new Dictionary<string, StringValues>());

        // Act
        var result = context.GetClientName();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetOrAddRid_ShouldReturnRequestId_WhenRequestIdExistsInRequestHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers.Add(HeaderName, "123456");

        // Act
        var result = context.GetOrAddRid();

        // Assert
        result.Should().Be("123456");
    }

    [Fact]
    public void GetOrAddRid_ShouldGenerateAndReturnRequestId_WhenRequestIdDoesNotExistInRequestHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var result = context.GetOrAddRid();

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GetOrAddRid_ShouldAddRequestIdToResponseHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        var result = context.GetOrAddRid();

        // Assert
        context.Response.Headers.Should().ContainKey(HeaderName);
        context.Response.Headers[HeaderName].Should().Contain(result);
    }
}