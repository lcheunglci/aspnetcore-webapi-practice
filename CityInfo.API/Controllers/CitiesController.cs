using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/cities")]
[ApiVersion(1)]
[ApiVersion(2)]
//[Authorize]
public class CitiesController(ICityInfoRepository cityInfoRepository, 
    IMapper mapper) : ControllerBase
{
    const int _maxCitiesPageSize = 20; 

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(string? name,
        string? searchQuery,
        int pageNumber = 1, 
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageSize > _maxCitiesPageSize)
        {
            pageSize = _maxCitiesPageSize;
        }

        var (cityEntities, paginationMetadata) = await cityInfoRepository.GetCitiesReadOnlyAsync(name,
            searchQuery,
            pageNumber,
            pageSize,
            cancellationToken);

        if (paginationMetadata != null)
        {
            Response.Headers.Append("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));
        }

        return Ok(mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities)); 
    }

	/// <summary>
	/// Get a city by cityId
	/// </summary>
	/// <param name="cityId">The cityId of the city to get</param>
	/// <param name="includePointsOfInterest">Whether or not to include the points of interest</param>
	/// <param name="cancellationToken">The injected cancellation token</param>
	/// <returns>A city with or without points of interest</returns>
    [HttpGet("{cityId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetCity(int cityId, 
        bool includePointsOfInterest = false,
        CancellationToken cancellationToken = default)
    {
        var city = await cityInfoRepository.GetCityAsync(cityId, 
            includePointsOfInterest, 
            cancellationToken);

        if (city == null)
        {
            return NotFound();
        }

        if (includePointsOfInterest)
        {
            return Ok(mapper.Map<CityDto>(city));
        }

        return Ok(mapper.Map<CityWithoutPointsOfInterestDto>(city));
    }
}
