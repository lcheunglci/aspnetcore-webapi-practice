using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping
          = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new PropertyMappingValue(new List<string> { "Id" } ) },
            { "MainCategory", new PropertyMappingValue(new List<string>() {"MainCategory"})},
            { "Age", new PropertyMappingValue(new List<string>() {"DataOfBirth"}, true)},
            { "Name", new PropertyMappingValue(new List<string>() {"FirstName", "LastName"})},
        };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();


        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestintation>()
        {
            // get matching mapping
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestintation>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;

            }

            throw new Exception($"Cannot find exact property instance" + $"for <{typeof(TSource)}, {typeof(TDestintation)}");
        }
    }
}
