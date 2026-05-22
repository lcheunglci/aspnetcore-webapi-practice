using CityInfo.API.Models;

namespace CityInfo.API
{
	public class CitiesDataStore
	{
		public List<CityDto> Cities { get; set; }

		public CitiesDataStore()
		{
			// init dummy data
			Cities = [
				new ()
				{
					Id = 1,
					Name = "New York City",
					Description = "The one with that big park.",
					PointsOfInterest = [
						new () {
							Id = 1,
							Name = "Central Park",
							Description = "The most visited urban park in the United States."
						},
						new () {
							Id = 2,
							Name = "Empire State Building",
							Description = "A 102-story skyscraper located in Midtown Manhattan."
						}
					]
				},
				new ()              {
					Id = 2,
					Name = "Tokyo",
					Description = "The land of the rising sun.",
					PointsOfInterest = [
						new () {
							Id = 1,
							Name = "Meiji Shrine",
							Description = "A Shinto shrine dedicated to the deified spirits of Emperor Meiji and his wife, Empress Shōken."
						},
						new () {
							Id = 2,
							Name = "Tokyo Tower",
							Description = "A communications and observation tower in the Shiba-koen district of Minato, Tokyo, Japan."
						}
					]
				}, new () {
					Id = 3,
					Name = "Paris",
					Description = "The one with that big tower.",
					PointsOfInterest = [
						new () {
							Id = 1,
							Name = "Eiffel Tower",
							Description = "A wrought iron lattice tower on the Champ de Mars, named after the engineer Gustave Eiffel."
						},
						new () {
							Id = 2,
							Name = "The Louvre",
							Description = "The world's largest museum."
						}
					]
				}
			];
		}
	}
}
