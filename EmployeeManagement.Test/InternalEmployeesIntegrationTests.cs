using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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

		[Fact]
		public async Task CreateInternalEmployee_ValidInput_MustReturn201WithLocationHeader()
		{
			// Arrange
			var employeeForCreation = new { FirstName = "Test", LastName = "Employee" };
			var content = new StringContent(JsonSerializer.Serialize(employeeForCreation));

			// Act
			var response = await _httpClient.PostAsync(
				"/api/internalemployees", content, TestContext.Current.CancellationToken);

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			var createdEmployee = await response.Content.ReadFromJsonAsync<InternalEmployee>(TestContext.Current.CancellationToken);
			Assert.NotNull(createdEmployee);
			Assert.Equal("Test", createdEmployee.FirstName);
			Assert.Equal("Employee", createdEmployee.LastName);
			Assert.Equal(2500, createdEmployee.Salary);
			Assert.NotEqual(Guid.Empty, createdEmployee.Id);
		}

		[Fact]
		public async Task CreateInternalEmployee_ValidInput_MustRoundTrip()
		{
			// Arrange
			var employeeForCreation = new { FirstName = "Roundtrip", LastName = "Test" };
			var content = new StringContent(JsonSerializer.Serialize(employeeForCreation), Encoding.UTF8, "application/json");

			// Act - create
			var createResponse = await _httpClient.PostAsync(
				"/api/internalemployees", content, TestContext.Current.CancellationToken);

			Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
			Assert.NotNull(createResponse.Headers.Location);

			// Assert
			var getResponse = await _httpClient.GetAsync(
				createResponse.Headers.Location, TestContext.Current.CancellationToken);

			var retrievedEmployee = await createResponse.Content.ReadFromJsonAsync<InternalEmployee>(TestContext.Current.CancellationToken);
			Assert.NotNull(retrievedEmployee);
			Assert.Equal("Roundtrip", retrievedEmployee.FirstName);
			Assert.Equal("Test", retrievedEmployee.LastName);
			Assert.Equal(2500, retrievedEmployee.Salary);
			Assert.NotEqual(Guid.Empty, retrievedEmployee.Id);
		}

		[Fact]
		public async Task GetInternalEmployee_NonExistentId_MustReturn404NotFound()
		{
			// Arrange
			var nonExistentId = Guid.NewGuid();

			// d
			var response = await _httpClient.GetAsync(
				$"/api/internalemployees/{nonExistentId}", TestContext.Current.CancellationToken);

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task GetInternalEmployees_WhenCalled_ResponseContentTypeMustBeJson()
		{
			// Arrange and Act
			var response = await _httpClient.GetAsync("/api/internalemployee",
				TestContext.Current.CancellationToken);

			// Assert
			response.EnsureSuccessStatusCode();
			Assert.Equal("application/json; charset=utf-8",
				response.Content.Headers.ContentType?.ToString());
		}

		[Fact]
		public async Task GetInternalEmployees_WhenCalled_MustContainSecurityHeaders()
		{
			// Arrange and Act
			var response = await _httpClient.GetAsync("/api/internalemployees",
				TestContext.Current.CancellationToken);

			// Assert
			response.EnsureSuccessStatusCode();

			Assert.True(response.Headers.Contains("X-Content-Type-Options"));
			Assert.Equal("nosniff",
				response.Headers.GetValues("X-Content-Type-Options").First());

			Assert.True(response.Headers.Contains("Content-Security-Policy"));
			Assert.Equal("default-src 'self';frame-ancestors 'none';",
				response.Headers.GetValues("Content-Security-Policy").First());
		}

		[Fact]
		public async Task GetInternalEmployees_AcceptHeaderXml_MustReturn406NotAcceptable()
		{
			// Arrange
			var request = new HttpRequestMessage(
				HttpMethod.Get, "/api/internalemployees");
			request.Headers.Accept.Add(
				new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml"));

			// Act
			var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

			// Assert
			Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
		}

		[Fact]
		public async Task GetNonExistentRoute_MustReturn404()
		{
			// Arrange & Act
			var response = await _httpClient.GetAsync("/api/nonexistent",
				TestContext.Current.CancellationToken);

			// Assert
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async Task CreateInternalEmployee_MissingFirstName_MustReturn400BadRequest()
		{
			// Arrange
			var invalidEmployee = new { LastName = "Employee" };
			var content = new StringContent(JsonSerializer.Serialize(invalidEmployee), Encoding.UTF8, "application/json");

			// Act
			var response = await _httpClient.PostAsync(
				"api/internalemployees", content, TestContext.Current.CancellationToken);

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

		}
	}
}
