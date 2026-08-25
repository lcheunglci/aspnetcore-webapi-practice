using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using EmployeeManagement.Test.Fixtures;

namespace EmployeeManagement.Test;

public class MinimalApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly HttpClient _httpClient;
	public MinimalApiIntegrationTests(CustomWebApplicationFactory factory)
	{
		_httpClient = factory.CreateClient();
	}

	[Fact]
	public async Task GetCourses_WhenCalled_MustReturn200OkWithCourses()
	{
		// Arrange (handled by constructor)

		// Act
		var response = await _httpClient.GetAsync("/api/courses", TestContext.Current.CancellationToken);

		// Assert
		response.EnsureSuccessStatusCode();
		var courses = await response.Content.ReadFromJsonAsync<List<CourseResponse>>(TestContext.Current.CancellationToken);
		Assert.NotNull(courses);
		Assert.True(courses.Count >= 5);
		Assert.Contains(courses, c => c.Title == "Company Introduction");
	}

	[Fact]
	public async Task CreateCourse_ValidInput_MustReturn201WithCreatedCourse()
	{
		// Arrange
		var courseForCreation = new { Title = "Integration Testing 101" };
		var content = new StringContent(JsonSerializer.Serialize(courseForCreation), Encoding.UTF8, "application/json");

		// Act
		var response = await _httpClient.PostAsync("/api/courses", content, TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		var createdCourse = await response.Content.ReadFromJsonAsync<CourseResponse>(TestContext.Current.CancellationToken);
		Assert.NotNull(createdCourse);
		Assert.Equal("Integration Testing 101", createdCourse.Title);
		Assert.NotEqual(Guid.Empty, createdCourse.Id);
	}

	[Fact]
	public async Task CreateCourse_MissingTitle_MustReturnValidationProblem()
	{
		// Arrange
		var courseForCreation = new { Title = "" };
		var content = new StringContent(JsonSerializer.Serialize(courseForCreation), Encoding.UTF8, "application/json");

		// Act
		var response = await _httpClient.PostAsync("/api/courses", content, TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	private record CourseResponse(Guid Id, string Title);
}

