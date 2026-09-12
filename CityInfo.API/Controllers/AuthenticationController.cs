using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		[HttpPost("authenticate")]
		public ActionResult<string> Authenticate(AuthenticationRequestDto authenticationRequest)
		{
			// Step 1: validate the username / password
			var user = ValidateUserCredentials(authenticationRequest.UserName, authenticationRequest.Password);

			if (user is null)
			{
				return Unauthorized();
			}

			var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["AuthenticateKey"]));
			var signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		}

		public CityInfoUser? ValidateUserCredentials(string? userName, string? password)
		{
			// we don't have a user DB or stable. If you have, check the passed-through
			// username/password against what's stored in the database.
			// 
			// For demo purposes, we assume the credentials are valid

			// return a new CityInfoUser (values would normally come from your user DB/table)
			return new CityInfoUser(1, userName ?? "", "Bob", "Doe", "Antwerp");

		}
	}
}
