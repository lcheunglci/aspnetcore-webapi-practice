using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentException(nameof(logger));
            _mailService = mailService ??
                throw new ArgumentException(nameof(mailService));
            _cityRepository = cityInfoRepository ??
                throw new ArgumentException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentException(nameof(mapper));
        }


        [HttpGet()]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest");
                    return NotFound();
                }

                var pointsOfInterestCity = _cityRepository.GetPointsOfInterestForCity(cityId);


                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestCity));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting point of interest with id {cityId}.", ex);
                return StatusCode(500, "A problem while handling your request.");
            }
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {
            if (!_cityRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }


        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId,
            [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_cityRepository.CityExists(cityId))
            {
                return NotFound();
            }


            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            // demo purposes to be improved
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            _cityRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);
            _cityRepository.Save();

            var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);


            return CreatedAtRoute("GetPointOfInterest", new { cityId, id = createdPointOfInterestToReturn.Id }, createdPointOfInterestToReturn);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForUpdateDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_cityRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityRepository.GetPointOfInterestForCity(cityId, id);


            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            _cityRepository.UpdatePointOfInterestForCity(cityId, pointOfInterestEntity);

            _cityRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (!_cityRepository.CityExists(cityId))
            {
                return NotFound();
            }


            var pointOfInterestEntity = _cityRepository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description",
                    "The provided description should be different from the name");
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest();
            }


            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            _cityRepository.UpdatePointOfInterestForCity(cityId, pointOfInterestEntity);

            _cityRepository.Save();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_cityRepository.CityExists(cityId))
            {
                return NotFound();
            }


            var pointOfInterestEntity = _cityRepository.GetPointOfInterestForCity(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }


            _cityRepository.DeletePointOfInterest(pointOfInterestEntity);

            _cityRepository.Save();

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();

        }


    }
}
