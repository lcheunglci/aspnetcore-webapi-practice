using System;
using System.Collections.Generic;
using System.Text;
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
	}
}
