using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blog.PublicAPI.Features.Users;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Categories;

namespace Blog.PublicAPI.Tests.IntegrationTests.Features.Users;

[IntegrationTest]
public class UsersControllerTests
{
    [Fact]
    public async Task Should_ReturnsHttp200Ok_When_Post_Valid_Request()
    {
        // Arrange
        await using var webApplicationFactory = new WebApplicationFactory<Program>();
        using var httpClient = webApplicationFactory.CreateClient();

        var request = new Faker<CreateUserRequest>()
            .CustomInstantiator(faker => new CreateUserRequest(faker.Person.UserName, faker.Person.Email, faker.Random.String2(4)))
            .Generate();

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
    public async Task Should_ReturnsHttp400BadRequest_When_Post_Invalid_Request()
    {
        // Arrange
        await using var webApplicationFactory = new WebApplicationFactory<Program>();
        using var httpClient = webApplicationFactory.CreateClient();

        var request = new CreateUserRequest(string.Empty, string.Empty, string.Empty);

        // Act
        using var act = await httpClient.PostAsJsonAsync("/api/users", request);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var response = await act.Content.ReadFromJsonAsync<ProblemDetails>();
        response.Title.Should().Be("One or more validation errors occurred.");
        response.Status.Should().Be(StatusCodes.Status400BadRequest);
        response.Extensions.Keys.Should().Contain("errors");
    }
}