using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();

        City GetCity(int cityId, bool includePointsOfInterest);

        IEnumerable<PointOfInterest> GetPointOfInterestsForCity(int cityId, bool includePointsOfInterest);

        PointOfInterest GetPointsOfInterestForCity(int cityId);


    }
}
