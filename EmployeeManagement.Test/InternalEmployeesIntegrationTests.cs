using System.Net.Http.Json;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Test.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EmployeeManagement.Test
{
	public class InternalEmployeesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
	{
		private readonly HttpClient _httpClient;

		public InternalEmployeesIntegrationTests(CustomWebApplicationFactory factory)
		{
			_httpClient = factory.CreateClient(
				new WebApplicationFactoryClientOptions()
				{
					AllowAutoRedirect = false
				});
		}

		[Fact]
		public async Task GetInternalEmployees_WhenCalled_MustReturn200Ok()
		{
			// Arrange (handled by the constructor)

			// Act
			var response = await _httpClient.GetAsync("/api/internalemployees", TestContext.Current.CancellationToken);

			// Assert
			response.EnsureSuccessStatusCode();
			var employees = await response.Content.ReadFromJsonAsync<List<InternalEmployee>>(TestContext.Current.CancellationToken);
			Assert.NotNull(employees);
			Assert.True(employees.Count >= 2);

		}
	}
}
