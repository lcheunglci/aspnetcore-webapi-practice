using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[ApiController]
	public class CitiesController : ControllerBase
	{
		[HttpGet("api/cities")]
		public JsonResult GetCtities()
		{
			return new JsonResult(new []
			{
				new
				{
					Id = 1,
					Name = "New York City",
					Description = "The one with that big park."
				},
				new
				{
					Id = 2,
					Name = "Tokyo",
					Description = "The land of the rising sun."
				},
				new
				{
					Id = 3,
					Name = "Paris",
					Description = "The one with that big tower."
				}
			});
		}
	}
}
