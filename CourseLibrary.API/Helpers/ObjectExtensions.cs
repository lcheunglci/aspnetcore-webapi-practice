using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers
{
	public static class ObjectExtensions
	{
		public static ExpandoObject ShapeData<TSource>(this TSource source, string? fields)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			var dataShapedObject = new ExpandoObject();

			if (string.IsNullOrWhiteSpace(fields))
			{
				// all public properties should be in ExpandoObject
				var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

				foreach (var propertyInfo in propertyInfos)
				{
					// get the value of the property on the source object
					var propertyValue = propertyInfo.GetValue(source);
					((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
				}

				return dataShapedObject;
			}
			var fieldsToInclude = fields.Split(',');
			foreach (var field in fieldsToInclude)
			{
				// trim each field, as it might contain leading or trailing spaces
				var propertyName = field.Trim();

				var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				if (propertyInfo == null)
				{
					throw new Exception($"Property '{propertyName}' does not exist on '{typeof(TSource)}'");
				}

				var propertyValue = propertyInfo.GetValue(source);
				((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
			}

			// return the list
			return dataShapedObject;
		}
	}
}

