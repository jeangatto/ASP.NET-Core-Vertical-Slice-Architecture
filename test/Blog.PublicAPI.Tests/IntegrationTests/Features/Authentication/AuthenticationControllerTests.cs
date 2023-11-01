using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blog.PublicAPI.Features.Authentication;
using Blog.PublicAPI.Features.Users;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Categories;

namespace Blog.PublicAPI.Tests.IntegrationTests.Features.Authentication;

[IntegrationTest]
public class AuthenticationControllerTests
{
    [Fact]
    public async Task Should_ReturnsHttpOk_When_PostValidRequest()
    {
        // Arrange
        var createUserRequest = new Faker<CreateUserRequest>()
            .CustomInstantiator(faker => new CreateUserRequest(faker.Person.UserName, faker.Person.Email, faker.Random.String2(4)))
            .Generate();

        await using var webApplicationFactory = new WebApplicationFactory<Program>();
        using var httpClient = webApplicationFactory.CreateClient();

        await httpClient.PostAsJsonAsync("/api/users", createUserRequest);

        var authRequest = new AuthenticationRequest(createUserRequest.Email, createUserRequest.Password);

        // Act
        var act = await httpClient.PostAsJsonAsync("api/auth", authRequest);

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await act.Content.ReadFromJsonAsync<TokenResponse>();
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.ExpiresIn.Should().BePositive();
    }

    [Fact]
    public async Task Should_ReturnsHttpBadRequest_When_PostInvalidRequest()
    {
        // Arrange
        var request = new AuthenticationRequest(string.Empty, string.Empty);

        await using var webApplicationFactory = new WebApplicationFactory<Program>();
        using var httpClient = webApplicationFactory.CreateClient();

        // Act
        using var act = await httpClient.PostAsJsonAsync("/api/auth", request);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var response = await act.Content.ReadFromJsonAsync<ProblemDetails>();
        response.Title.Should().Be("One or more validation errors occurred.");
        response.Status.Should().Be(StatusCodes.Status400BadRequest);
        response.Extensions.Keys.Should().Contain("errors");
    }
}