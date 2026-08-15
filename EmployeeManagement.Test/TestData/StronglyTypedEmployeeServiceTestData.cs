using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Test.TestData
{
	public class StronglyTypedEmployeeServiceTestData : TheoryData<int, bool>
	{
		public StronglyTypedEmployeeServiceTestData()
		{
			Add(100, true);
			Add(200, false);
		}
	}
}
