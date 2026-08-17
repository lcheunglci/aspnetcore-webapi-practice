using EmployeeManagement.Business;
using EmployeeManagement.Business.Exceptions;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Test.Fixtures;
using Moq;

namespace EmployeeManagement.Test
{
	public class EmployeeServiceTests : IClassFixture<EmployeeServiceFixture>
	{
		private readonly EmployeeServiceFixture _employeeServiceFixture;

		public EmployeeServiceTests(EmployeeServiceFixture employeeServiceFixture)
		{
			_employeeServiceFixture = employeeServiceFixture;
		}

		[Fact]
		public async Task GiveRaiseBelowMinimumGiven_EmployeeInvalidRaiseExceptionMustBeThrown()
		{
			// Arrange
			var internalEmployee = new InternalEmployee("John", "Doe", 5, 3000, false, 1);

			// Act & Assert
			await Assert.ThrowsAsync<EmployeeInvalidRaiseException>(
				async () => await _employeeServiceFixture.EmployeeService.GiveRaiseAsync(internalEmployee, 50));
		}


		[Fact]
		public async Task FetchInternalEmployee_EmployeeFetched_SuggestedBonusMustBeCalculated()
		{
			// Arrange (done in constructor with fixture)
		
			// Act
			var employee = await _employeeServiceFixture.EmployeeService.FetchInternalEmployeeAsync(Guid.Empty);

			// Assert
			Assert.NotNull(employee);
			Assert.Equal(400, employee.SuggestedBonus);
		}


	}
}
