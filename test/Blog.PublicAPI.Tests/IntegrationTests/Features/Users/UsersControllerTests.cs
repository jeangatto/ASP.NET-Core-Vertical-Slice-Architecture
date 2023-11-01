using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.UserAggregate;
using Blog.PublicAPI.Features.Users;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Categories;

namespace Blog.PublicAPI.Tests.IntegrationTests.Features.Users;

[IntegrationTest]
public class UsersControllerTests
{
    [Fact]
    public async Task Post_ValidRequestBody_ReturnsHttpOk()
    {
        // Arrange
        var request = new Faker<CreateUserRequest>()
            .RuleFor(request => request.Name, faker => faker.Person.UserName)
            .RuleFor(request => request.Email, faker => faker.Person.Email)
            .RuleFor(request => request.Password, faker => faker.Random.String2(4))
            .Generate();

        await using var webApplicationFactory = new WebApplicationFactory<Program>();

        using var httpClient = webApplicationFactory.CreateClient();

        // Act
        using var act = await httpClient.PostAsJsonAsync("/api/users", request);

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await act.Content.ReadFromJsonAsync<UserResponse>();
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();
        response.Name.Should().NotBeNullOrWhiteSpace();
        response.Email.Should().NotBeNullOrWhiteSpace();
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Post_InvalidRequestBody_ReturnsHttpBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest(string.Empty, string.Empty, string.Empty);

        await using var webApplicationFactory = new WebApplicationFactory<Program>();

        using var httpClient = webApplicationFactory.CreateClient();

        // Act
        using var act = await httpClient.PostAsJsonAsync("/api/users", request);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var response = await act.Content.ReadFromJsonAsync<ProblemDetails>();
        response.Title.Should().Be("One or more validation errors occurred.");
        response.Status.Should().Be(StatusCodes.Status400BadRequest);
        response.Extensions.Keys.Should().Contain("errors");
    }

    [Fact]
    public async Task Post_AlreadyExistingUser_ReturnsHttpConflict()
    {
        // Arrange
        var user = new Faker<User>()
            .CustomInstantiator(faker => new User(faker.Person.UserName, faker.Person.Email, faker.Internet.Password()))
            .Generate();

        var request = new Faker<CreateUserRequest>()
            .RuleFor(request => request.Name, user.Name)
            .RuleFor(request => request.Email, user.Email)
            .RuleFor(request => request.Password, faker => faker.Random.String2(4))
            .Generate();

        await using var webApplicationFactory = new WebApplicationFactory<Program>();
        await using var scope = webApplicationFactory.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        await dbContext.AddAsync(user);
        await dbContext.SaveChangesAsync();
        using var httpClient = webApplicationFactory.CreateClient();

        // Act
        using var act = await httpClient.PostAsJsonAsync("/api/users", request);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var response = await act.Content.ReadFromJsonAsync<ProblemDetails>();
        response.Title.Should().Be("There was a conflict.");
        response.Detail.Should().ContainAll("Next error(s) occured:* The email address provided is already in use.");
        response.Status.Should().Be(StatusCodes.Status409Conflict);
    }
}