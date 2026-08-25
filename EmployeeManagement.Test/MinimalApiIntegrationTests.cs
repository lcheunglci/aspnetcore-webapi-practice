using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
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

	private record CourseResponse(Guid id, string title);
}

