using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<T> source, string? fields)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			// create a list to hold our ExpandoObjects
			var expandoObjectList = new List<ExpandoObject>();

			// Create a list with PropertyInfo objects on TSource. Reflection is
			// expensive, so rather than doing it for each object in the list, we do
			// it once and reuse the results. After all, part of the reflection is on
			// type of the object (TSource), not on the instance
			var propertyInfoList = new List<PropertyInfo>();

			if (string.IsNullOrWhiteSpace(fields))
			{
				// all public properties should be in the ExpandoObject
				var propertyInfos = typeof(TSource)
					.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				propertyInfoList.AddRange(propertyInfos);
			}
			else
			{
				// the fields are separated by ",", so we split it.
				var fieldsAfterSplit = fields.Split(',', StringSplitOptions.RemoveEmptyEntries);
				// now we need to find the properties for the specified fields
				foreach (var field in fieldsAfterSplit)
				{
					// trim each field, as it might contain leading
					// or trailing spaces. Can't trim the var in foreach,
					// so use another var.
					var propertyName = field.Trim();
					// use reflection to get the property on the source object
					// we need to include public and instance, because specifying
					// a binding flag overwrites the already-existing binding flags.
					// ignore case.
					var propertyInfo = typeof(TSource)
						.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
					if (propertyInfo == null)
					{
						throw new Exception($"Property {propertyName} wasn't found on " +
							$"{typeof(TSource)}");
					}
					propertyInfoList.Add(propertyInfo);
				}
			}

			// run through the source objects
			foreach (var sourceObject in source)
			{
				// create an ExpandoObject that will hold the
				// selected properties & values
				var dataShapedObject = new ExpandoObject();
				// get the value of each property we have to return
				foreach (var propertyInfo in propertyInfoList)
				{
					var propertyValue = propertyInfo.GetValue(sourceObject);
					// add the field to the ExpandoObject
					((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
				}
				// add the ExpandoObject to the list
				expandoObjectList.Add(dataShapedObject);
			}

			// return the list
			return expandoObjectList;

		}
	}
}
