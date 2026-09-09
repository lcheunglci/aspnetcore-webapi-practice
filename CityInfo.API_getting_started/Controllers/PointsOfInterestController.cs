using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityId}/pointsofinterest")]
	[ApiController]
	public class PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, CitiesDataStore citiesDataStore) : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<Models.PointOfInterestDto>> GetPointsOfInterest(int cityId)
		{
			// HttpContext.RequestServices.GetService()
			var city = citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				logger.LogInformation("City with id {cityId} was not found when accessing points of interest.", cityId);
				return NotFound();
			}
			return Ok(city.PointsOfInterest);
		}

		[HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
		public ActionResult<Models.PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId, CitiesDataStore citiesDataStore)
		{
			var city = citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
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
			var city = citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}
			var maxPointOfInterestId = citiesDataStore.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
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
			var city = citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
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
			mailService.Send("Point of interest deleted.",
				$"Point of interest {pointOfInterest.Name} with id {pointOfInterest.Id} was deleted.");

			return NoContent();
		}

		[HttpPut("{pointOfInterestId}")]
		public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
			PointOfInterestForUpdateDto pointOfInterest)
		{
			var city = citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null)
			{
				return NotFound();
			}

			// find point of interest
			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterestFromStore is null)
			{
				return NotFound();
			}
			pointOfInterestFromStore.Name = pointOfInterest.Name;
			pointOfInterestFromStore.Description = pointOfInterest.Description;
			return NoContent();
		}

		[HttpPatch("{pointOfInterestId}")]
		public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
			JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
		{
			var city = citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city is null)
			{
				return NotFound();
			}
			// find point of interest
			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
			if (pointOfInterestFromStore is null)
			{
				return NotFound();
			}
			var pointOfInterestToPatch = new PointOfInterestForUpdateDto
			{
				Name = pointOfInterestFromStore.Name,
				Description = pointOfInterestFromStore.Description
			};
			patchDocument.ApplyTo(pointOfInterestToPatch, jsonPatchError =>
			{
				var key = jsonPatchError.AffectedObject.GetType().Name;
				ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
			});

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (!TryValidateModel(pointOfInterestToPatch))
			{
				return BadRequest(ModelState);
			}

			pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
			pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
			return NoContent();
		}
	}
}
