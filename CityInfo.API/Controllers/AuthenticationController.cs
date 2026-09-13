using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController(IConfiguration configuration) : ControllerBase
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

			var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration["AuthenticateKey:SecretForKey"]));
			var signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claimsForToken = new List<Claim> {
				new("sub", user.UserId.ToString()),
				new("given_name", user.FirstName),
				new("family_name", user.LastName),
				new("city", user.City),
			};

			var jwtSecurityToken = new JwtSecurityToken(
				configuration["AuthenticateKey:Issuer"],
				configuration["AuthenticateKey:Audience"],
				claimsForToken,
				DateTime.UtcNow,
				DateTime.UtcNow.AddMinutes(30),
				signingCredential
			);

			var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

			return Ok(new { token = tokenToReturn });
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
