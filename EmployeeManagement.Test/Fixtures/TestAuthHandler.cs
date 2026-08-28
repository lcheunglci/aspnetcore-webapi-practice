using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Test.Fixtures
{
	public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public const string SchemeName = "TestScheme";

		public static IList<Claim> Claims { get; set; } = new List<Claim>()
		{
			new Claim(ClaimTypes.Name, "TestUser"),
			new Claim(ClaimTypes.Role, "Admin"),
		};

		public TestAuthHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder) : base(options, logger, encoder)
		{
		}


		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var identity = new ClaimsIdentity(Claims, SchemeName);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, SchemeName);

			return Task.FromResult(AuthenticateResult.Success(ticket));
		}
	}
}
