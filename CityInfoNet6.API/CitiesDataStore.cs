using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            // init dummy data
            Cities = new List<CityDto>()
            {
                new CityDto() { Id = 1, Name ="New York City", Description="The one with that big park", PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto() {
                        Id = 1, Name = "Central Park", Description = "The most visited urban park in the United States."
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2, Name = "Empire State building", Description="A 102-story skyscraper located in midtown Manhattan."
                    }
                } },
                new CityDto() { Id = 2, Name ="Tokyo", Description="The one with that big kaiju", PointsOfInterest = new List<PointOfInterestDto>() {
                    new PointOfInterestDto() {
                        Id = 1, Name = "Shibuya", Description = "The one with the crosswalk that works in every direction"
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2, Name = "Ebisu", Description = "The one with the Museum of Yebisu Beer."
                    }
                } },
                new CityDto() { Id = 3, Name ="Paris", Description="The one with that big tower", PointsOfInterest = new List<PointOfInterestDto>() {
                    new PointOfInterestDto() {
                        Id = 1, Name = "Eiffle tower", Description = "The tall pointy thing "
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2, Name = "Louvre Museum", Description="Looks like an apple store in Mission Impossible."
                    }
                } }
            };
        }
    }
}
