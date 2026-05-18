using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityId}/pointsofinterest")]
	[ApiController]
	public class PointsOfInterestController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<Models.PointOfInterestDto>> GetPointsOfInterest(int cityId)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}
			return Ok(city.PointsOfInterest);
		}

		[HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
		public ActionResult<Models.PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterest == null)
			{
				return NotFound();
			}

			return Ok(pointOfInterest);
		}

		[HttpPost]
		public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
			PointOfInterestForCreationDto pointOfInterst)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}
			var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
			var finalPointOfInterest = new PointOfInterestDto
			{
				Id = ++maxPointOfInterestId,
				Name = pointOfInterst.Name,
				Description = pointOfInterst.Description
			};
			city.PointsOfInterest.Add(finalPointOfInterest);
			return CreatedAtRoute("GetPointOfInterest",
				new { cityId, pointOfInterestId = finalPointOfInterest.Id }, finalPointOfInterest);
		}

		[HttpDelete("{pointOfInterestId}")]
		public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}
			var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterest == null)
			{
				return NotFound();
			}
			city.PointsOfInterest.Remove(pointOfInterest);
			return NoContent();
		}
	}
}
