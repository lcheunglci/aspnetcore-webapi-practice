namespace CityInfo.API.Models;

public class PointOfInterestCreationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public PointOfInterestDto? PointOfInterest { get; set; }

    public static PointOfInterestCreationResult Successful(PointOfInterestDto poi) =>
        new()
        {
            Success = true,
            PointOfInterest = poi
        };

    public static PointOfInterestCreationResult Failed(string error) =>
        new()
        {
            Success = false,
            ErrorMessage = error
        };
}
