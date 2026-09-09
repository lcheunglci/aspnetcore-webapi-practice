using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController(CitiesDataStore citiesDataStore) : ControllerBase
	{
		[HttpGet()]
		public ActionResult<IEnumerable<CityDto>> GetCities()
		{
			return Ok(citiesDataStore.Cities);
		}


		[HttpGet("{id}")]
		public ActionResult<CityDto> GetCity(int id)
		{
			var cityToReturn = citiesDataStore.Cities.FirstOrDefault(c => c.Id == id);

			if (cityToReturn == null) {
				return NotFound();
			}

			return Ok(cityToReturn);
		}
	}
}
