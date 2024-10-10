using System;
using FluentAssertions;
using Maersk.FbM.OCT.Extensions;
using Xunit;

namespace Maersk.FbM.OCT.Controller.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void Sanitize_ShouldRemoveNewLineCharacters()
    {
        // Arrange
        const string input = "This is a string with\nnew line characters.";

        // Act
        var sanitizedString = input.Sanitize();

        // Assert
        sanitizedString.Should().NotContain(Environment.NewLine);
    }

    [Fact]
    public void Sanitize_ShouldNotModifyStringWithoutNewLineCharacters()
    {
        // Arrange
        const string input = "This is a safe string without new line characters.";

        // Act
        var sanitizedString = input.Sanitize();

        // Assert
        sanitizedString.Should().Be(input);
    }

    [Fact]
    public void Sanitize_ShouldHandleEmptyInput()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var sanitizedString = input.Sanitize();

        // Assert
        sanitizedString.Should().BeEmpty();
    }
}