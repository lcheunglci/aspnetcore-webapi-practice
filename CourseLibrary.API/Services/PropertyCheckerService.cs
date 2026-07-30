namespace CourseLibrary.API.Services
{
	public class PropertyCheckerService : IPropertyCheckerService
	{
		public bool TypeHasProperties<T>(string? fields)
		{
			if (string.IsNullOrWhiteSpace(fields))
			{
				return true;
			}

			// The fields are separated by ",", so we split it.
			var fieldsAfterSplit = fields.Split(',');

			// check if the requested fields exist on source
			foreach (var field in fieldsAfterSplit)
			{
				// trim each field, as it might contain leading or trailing spaces
				// Can't trim the var in foreach so use another variable
				var propertyName = field.Trim();

				// use reflection to check if the property can be found on T
				var propertyInfo = typeof(T).GetProperty(propertyName,
					System.Reflection.BindingFlags.IgnoreCase |
					System.Reflection.BindingFlags.Public |
					System.Reflection.BindingFlags.Instance);

				// it can't be found, return false
				if (propertyInfo == null)
				{
					return false;
				}
			}
			return true;
		}
	}
}
