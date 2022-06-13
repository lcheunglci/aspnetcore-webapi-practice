using CourseLibrary.API.Services;

namespace CourseLibrary.API.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            // the orderBy string is separated by ",", so we split it.
            var orderByAfterSplit = orderBy.Split(',');

            // apply each orderby clause in reverse order - otherwise, the 
            // IQueryable will be ordered in the wrong order
            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                // trim the order by clause, as it might contain certain leading
                // or trailing spaces. Cannot trim the var in the foreach,
                // so use another 
                var trimmedOrderByClause = orderByClause.Trim();

                // if the sort ends with "desc", we order descending, otherwise, ascending
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // remove " asc"or "desc" from the orderByClause, so we
                // get the property name to look for in the mapping directory
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                // find the matching property
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException("Key mapping for {propertyName} = is missing.");

                }


            }


        }

    }
}
