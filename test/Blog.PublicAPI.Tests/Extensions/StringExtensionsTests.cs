using System;
using Blog.PublicAPI.Extensions;
using FluentAssertions;
using Xunit;

namespace Blog.PublicAPI.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("HOW TO CREATE A URL AND SEO-FRIENDLY STRING IN C# (TEXT TO SLUG GENERATOR)", "how-to-create-a-url-and-seo-friendly-string-in-c-text-to-slug-generator")]
    [InlineData("Vertical Slice Architecture using .NET5, CQRS, MediatR, EF Core, C#", "vertical-slice-architecture-using-net5-cqrs-mediatr-ef-core-c")]
    [InlineData("C# `float` vs `double`: Performance Considerations", "c-float-vs-double-performance-considerations")]
    [InlineData("Dangerous Using Declaration (C# 8.0)", "dangerous-using-declaration-c-8-0")]
    public void Should_ReturnsFriendyUrl_ForTitle(string title, string expected)
    {
        // Act
        var act = title.ToUrlFriendly();

        // Assert
        act.Should().NotBeNullOrWhiteSpace().And.Be(expected);
    }

    [Fact]
    public void Should_ThrowArgumentNullException_When_TitleIsNull()
    {
        // Arrange
        string title = null;

        // Act
        var act = title.ToUrlFriendly;

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Should_ThrowArgumentException_When_TitleIsEmpty()
    {
        // Arrange
        var title = string.Empty;

        // Act
        var act = title.ToUrlFriendly;

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}