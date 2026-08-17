using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test
{
	public class EmployeeFactoryTests : IDisposable
	{
		private EmployeeFactory _employeeFactory;

		public EmployeeFactoryTests()
		{
			_employeeFactory = new EmployeeFactory();
		}

		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500()
		{
			// Arrange (handled by constructor)

			// Act
			var employee = (InternalEmployee)_employeeFactory.CreateEmployee("John", "Doe");

			// Assert
			Assert.Equal(2500, employee.Salary);
		}

		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500And3000()
		{
			// Arrange (handled by constructor)

			// Act
			var employee = (InternalEmployee)_employeeFactory.CreateEmployee("John", "Doe");

			// Assert
			Assert.InRange(employee.Salary, 2500, 3000);
		}

		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeExternalEmployee()
		{
			// Arrange (handled by constructor)

			// Act
			var employee = _employeeFactory.CreateEmployee("John", "Doe", "Marvin", true);

			// Assert
			Assert.IsType<ExternalEmployee>(employee);
		}


		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeInternalEmployee()
		{
			// Arrange (handled by constructor)

			// Act
			var employee = _employeeFactory.CreateEmployee("John", "Doe");

			// Assert
			Assert.IsType<InternalEmployee>(employee);
		}

		[Fact]
		public void CreateEmployee_EmptyFirstName_ThrowsArgumentException()
		{
			// Arrange (handled by constructor)
			

			// Act and Asset
			Assert.Throws<ArgumentException>(() =>
			{
				_employeeFactory.CreateEmployee("", "Doe");
			});
		}

		public void Dispose()
		{
			// clean up setup code if needed
		}
	}
}
