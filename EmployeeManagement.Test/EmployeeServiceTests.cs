using EmployeeManagement.Business;
using EmployeeManagement.Business.Exceptions;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using Moq;

namespace EmployeeManagement.Test
{
	public class EmployeeServiceTests
	{
		[Fact]
		public async Task GiveRaiseBelowMinimumGiven_EmployeeInvalidRaiseExceptionMustBeThrown()
		{
			// Arrange
			var employeeManagementRepositoryMock = new Mock<IEmployeeManagementRepository>();

			var employeeService = new EmployeeService(employeeManagementRepositoryMock.Object, new EmployeeFactory());

			var internalEmployee = new InternalEmployee("John", "Doe", 5, 3000, false, 1);

			// Act & Assert
			await Assert.ThrowsAsync<EmployeeInvalidRaiseException>(
				async () => await employeeService.GiveRaiseAsync(internalEmployee, 50));
		}


		[Fact]
		public async Task FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculated()
		{
			// Arrange
			var employeeManagementRepositoryMock = new Mock<IEmployeeManagementRepository>();

			employeeManagementRepositoryMock.Setup(
				m => m.GetInternalEmployeeAsync(It.IsAny<Guid>()))
				.ReturnsAsync(new InternalEmployee("Tony", "Hall", 2, 2500, false, 2)
				{
					AttendedCourses = new List<Course>
				{
					new Course("A course"),
					new Course("Another course")
				}
				});

			var employeeService = new EmployeeService(employeeManagementRepositoryMock.Object, new EmployeeFactory());

			// Act
			var employee = await employeeService.FetchInternalEmployeeAsync(Guid.Empty);

			// Assert
			Assert.NotNull(employee);
			Assert.Equal(400, employee.SuggestedBonus);
		}


	}
}
