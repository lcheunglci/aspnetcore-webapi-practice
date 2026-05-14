using CityInfo.API.Models;

namespace CityInfo.API
{
	public class CitiesDataStore
	{
		public static CitiesDataStore Current { get; } = new();

		public List<CityDto> Cities { get; set; }

		public CitiesDataStore()
		{
			// init dummy data
			Cities = [
				new ()
				{
					Id = 1,
					Name = "New York City",
					Description = "The one with that big park."
				},
				new ()              {
					Id = 2,
					Name = "Tokyo",
					Description = "The land of the rising sun."
				}, new () {
					Id = 3,
					Name = "Paris",
					Description = "The one with that big tower."
				}
			];
		}
	}
}
