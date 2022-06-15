namespace CourseLibrary.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestintation>();

        bool ValidMappingExistsFor<TSource, TDestination>(string fields);

    }
}