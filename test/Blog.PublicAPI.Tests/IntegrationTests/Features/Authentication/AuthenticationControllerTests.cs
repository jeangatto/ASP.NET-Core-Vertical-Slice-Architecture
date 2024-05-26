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
    public async Task Post_ValidRequestBody_ReturnsHttpOk()
    {
        // Arrange
        var createUserRequest = new Faker<CreateUserRequest>()
            .RuleFor(request => request.Name, faker => faker.Person.UserName)
            .RuleFor(request => request.Email, faker => faker.Person.Email)
            .RuleFor(request => request.Password, faker => faker.Random.String2(4))
            .Generate();

        var authRequest = new AuthenticationRequest(createUserRequest.Email, createUserRequest.Password);

        await using var webApplicationFactory = new WebApplicationFactory<Program>();

        using var httpClient = webApplicationFactory.CreateClient();

        await httpClient.PostAsJsonAsync("/api/users", createUserRequest);

        // Act
        using var act = await httpClient.PostAsJsonAsync("api/auth", authRequest);

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await act.Content.ReadFromJsonAsync<TokenResponse>();
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.ExpiresIn.Should().BePositive();
    }

    [Fact]
    public async Task Post_InvalidRequestBody_ReturnsHttpBadRequest()
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

    [Fact]
    public async Task Post_NonExistentUser_ReturnsHttpNotFound()
    {
        // Arrange
        var request = new Faker<AuthenticationRequest>()
            .RuleFor(request => request.Email, faker => faker.Person.Email)
            .RuleFor(request => request.Password, faker => faker.Random.String2(4))
            .Generate();

        await using var webApplicationFactory = new WebApplicationFactory<Program>();

        using var httpClient = webApplicationFactory.CreateClient();

        // Act
        using var act = await httpClient.PostAsJsonAsync("/api/auth", request);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var response = await act.Content.ReadFromJsonAsync<ProblemDetails>();
        response.Title.Should().Be("Resource not found.");
        response.Detail.Should().ContainAny("User not found.");
        response.Status.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task Post_IncorrectPassword_ReturnsHttpUnprocessableEntity()
    {
        // Arrange
        var createUserRequest = new Faker<CreateUserRequest>()
            .RuleFor(request => request.Name, faker => faker.Person.UserName)
            .RuleFor(request => request.Email, faker => faker.Person.Email)
            .RuleFor(request => request.Password, faker => faker.Random.String2(4))
            .Generate();

        var authRequest = new Faker<AuthenticationRequest>()
            .RuleFor(request => request.Email, createUserRequest.Email)
            .RuleFor(request => request.Password, faker => faker.Random.String2(4))
            .Generate();

        await using var webApplicationFactory = new WebApplicationFactory<Program>();

        using var httpClient = webApplicationFactory.CreateClient();

        await httpClient.PostAsJsonAsync("/api/users", createUserRequest);

        // Act
        using var act = await httpClient.PostAsJsonAsync("/api/auth", authRequest);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var response = await act.Content.ReadFromJsonAsync<ProblemDetails>();
        response.Title.Should().Be("Something went wrong.");
        response.Detail.Should().ContainAny("Email or password is incorrect.");
        response.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }
}