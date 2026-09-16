using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCity(int id, 
        bool includePointsOfInterest = false,
        CancellationToken cancellationToken = default)
    {
        var city = await cityInfoRepository.GetCityAsync(id, 
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
