using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Test
{
	public class EmployeeFactoryTests
	{
		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBe2500()
		{
			// Arrange
			var employeeFactory = new EmployeeFactory();

			// Act
			var employee = (InternalEmployee)employeeFactory.CreateEmployee("John", "Doe");

			// Assert
			Assert.Equal(2500, employee.Salary);
		}

		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeBetween2500And3000()
		{
			// Arrange
			var employeeFactory = new EmployeeFactory();

			// Act
			var employee = (InternalEmployee)employeeFactory.CreateEmployee("John", "Doe");

			// Assert
			Assert.InRange(employee.Salary, 2500, 3000);
		}

		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeExternalEmployee()
		{
			// Arrange
			var employeeFactory = new EmployeeFactory();

			// Act
			var employee = employeeFactory.CreateEmployee("John", "Doe", "Marvin", true);

			// Assert
			Assert.IsType<ExternalEmployee>(employee);
		}


		[Fact]
		public void CreateEmployee_ConstructInternalEmployee_SalaryMustBeInternalEmployee()
		{
			// Arrange
			var employeeFactory = new EmployeeFactory();

			// Act
			var employee = employeeFactory.CreateEmployee("John", "Doe");

			// Assert
			Assert.IsType<InternalEmployee>(employee);
		}

		[Fact]
		public void CreateEmployee_EmptyFirstName_ThrowsArgumentException()
		{
			// Arrange
			var employeeFactory = new EmployeeFactory();

			// Act and Asset
			Assert.Throws<ArgumentException>(() =>
			{
				employeeFactory.CreateEmployee("", "Doe");
			});
		}
	}
}
