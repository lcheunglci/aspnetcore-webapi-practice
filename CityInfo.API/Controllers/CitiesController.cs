using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController : ControllerBase
	{
		[HttpGet()]
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
