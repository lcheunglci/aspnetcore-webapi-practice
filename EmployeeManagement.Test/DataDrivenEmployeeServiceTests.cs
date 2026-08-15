using System;
using System.Collections.Generic;
using System.Text;
using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Test.TestData;
using Moq;

namespace EmployeeManagement.Test
{
	public class DataDrivenEmployeeServiceTests
	{
		[Theory]
		[InlineData(100, true)]
		[InlineData(200, false)]
		public async Task GiveRaise_RaiseGiven_EmployeeMinimumRaiseGivenMatchesValue_WithInlineData(
			int raiseGiven, bool expectedValueForMinimumRaiseGiven)
		{
			// Arrange
			var employeeManagementRepositoryMock = new Mock<IEmployeeManagementRepository>();

			employeeManagementRepositoryMock.Setup(m => m.SaveChangesAsync()).Returns(Task.CompletedTask);

			var employeeService = new EmployeeService(employeeManagementRepositoryMock.Object, new EmployeeFactory());

			var internalEmployee = new InternalEmployee("John", "Doe", 5, 3000, false, 1);


			// Act
			await employeeService.GiveRaiseAsync(internalEmployee, raiseGiven);

			// Assert
			Assert.Equal(expectedValueForMinimumRaiseGiven, internalEmployee.MinimumRaiseGiven);
		}

		[Theory]
		[ClassData(typeof(StronglyTypedEmployeeServiceTestData))]
		public async Task GiveRaise_RaiseGiven_EmployeeMinimumRaiseGivenMatchesValue_WithInClassData(
			int raiseGiven, bool expectedValueForMinimumRaiseGiven)
		{
			// Arrange
			var employeeManagementRepositoryMock = new Mock<IEmployeeManagementRepository>();

			employeeManagementRepositoryMock.Setup(m => m.SaveChangesAsync()).Returns(Task.CompletedTask);

			var employeeService = new EmployeeService(employeeManagementRepositoryMock.Object, new EmployeeFactory());

			var internalEmployee = new InternalEmployee("John", "Doe", 5, 3000, false, 1);


			// Act
			await employeeService.GiveRaiseAsync(internalEmployee, raiseGiven);

			// Assert
			Assert.Equal(expectedValueForMinimumRaiseGiven, internalEmployee.MinimumRaiseGiven);
		}

		[Theory]
		[ClassData(typeof(StronglyTypedEmployeeServiceTestData_FromFile))]
		public async Task GiveRaise_RaiseGiven_EmployeeMinimumRaiseGivenMatchesValue_WithFileData(
			int raiseGiven, bool expectedValueForMinimumRaiseGiven)
		{
			// Arrange
			var employeeManagementRepositoryMock = new Mock<IEmployeeManagementRepository>();

			employeeManagementRepositoryMock.Setup(m => m.SaveChangesAsync()).Returns(Task.CompletedTask);

			var employeeService = new EmployeeService(employeeManagementRepositoryMock.Object, new EmployeeFactory());

			var internalEmployee = new InternalEmployee("John", "Doe", 5, 3000, false, 1);


			// Act
			await employeeService.GiveRaiseAsync(internalEmployee, raiseGiven);

			// Assert
			Assert.Equal(expectedValueForMinimumRaiseGiven, internalEmployee.MinimumRaiseGiven);
		}
	}
}
