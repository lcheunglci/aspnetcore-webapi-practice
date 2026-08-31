using EmployeeManagement.Middleware;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.Test
{
	public class MiddlewareTests
	{

		[Fact]
		public async Task SecurityHeaderMiddleware_InvokeAsync_MustAddContentSecurityPolicyHeader()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();
			RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
			var middleware = new EmployeeManagementSecurityHeadersMiddleware(next);

			// Act
			await middleware.InvokeAsync(httpContext);

			// Assert
			Assert.True(httpContext.Response.Headers.ContainsKey("Content-Security-Policy"));
			Assert.Equal("default-src 'self';frame-ancestors 'none'",
				httpContext.Response.Headers.ContentSecurityPolicy.ToString());
		}

		[Fact]
		public async Task SecurityHeaderMiddleware_InvokeAsync_MustAddXContentTypeOptionsHeader()
		{
			// Arrange
			var httpContext = new DefaultHttpContext();
			RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
			var middleware = new EmployeeManagementSecurityHeadersMiddleware(next);

			// Act
			await middleware.InvokeAsync(httpContext);

			// Assert
			Assert.True(httpContext.Response.Headers.ContainsKey("X-Content-Type-Options"));
			Assert.Equal("nosniff",
				httpContext.Response.Headers.XContentTypeOptions.ToString());
		}
	}
}
