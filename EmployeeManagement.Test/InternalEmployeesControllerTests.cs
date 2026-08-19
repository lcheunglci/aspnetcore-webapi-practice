using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Text;
using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.MapperProfiles;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Test
{
	public class InternalEmployeesControllerTests
	{
		[Fact]
		public async Task GetInternalEmployees_GetAction_MustReturnOkObjectResult()
		{
			// Arrange
			var employeeServiceMock = new Mock<IEmployeeService>();
			employeeServiceMock
				.Setup(m => m.FetchInternalEmployeesAsync())
				.ReturnsAsync(new List<InternalEmployee>() {
					new InternalEmployee( "John", "Doe", 2, 3000, false, 2 ),
					new InternalEmployee( "Jane", "Smith", 3, 3400, true, 1 )
				});

			var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeProfile>(), new LoggerFactory());
			var mapper = new Mapper(mapperConfiguration);

			var controller = new InternalEmployeesController(employeeServiceMock.Object, mapper);

			// Act
			var result = await controller.GetInternalEmployees();

			// Assert
			var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
			var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var dtos = Assert.IsAssignableFrom<IEnumerable<InternalEmployeeDto>>(okObjectResult.Value);
			Assert.Equal(2, dtos.Count());
		}

		[Fact]
		public async Task GetInteralEmployees_GetAction_MustReturnsCorrectlyMappedFirstEmployee()
		{
			// Arrange
			var firstEmployee = new InternalEmployee("John", "Doe", 2, 3000, false, 2)
			{
				Id = Guid.Parse("bfdd0acd-d314-491c-b6fa-0e1781e73700"),
				SuggestedBonus = 400
			};

			var employeeServiceMock = new Mock<IEmployeeService>();
			employeeServiceMock
				.Setup(m => m.FetchInternalEmployeesAsync())
				.ReturnsAsync(new List<InternalEmployee>() {
					firstEmployee,
				});

			var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeProfile>(), new LoggerFactory());
			var mapper = new Mapper(mapperConfiguration);

			var controller = new InternalEmployeesController(employeeServiceMock.Object, mapper);

			// Act
			var result = await controller.GetInternalEmployees();

			// Assert
			var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
			var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var dtos = Assert.IsAssignableFrom<IEnumerable<InternalEmployeeDto>>(okObjectResult.Value);
			var firstDto = dtos.First();
			
			Assert.Equal(firstEmployee.Id, firstDto.Id);
			Assert.Equal(firstEmployee.FirstName, firstDto.FirstName);
			Assert.Equal(firstEmployee.LastName, firstDto.LastName);
			Assert.Equal(firstEmployee.Salary, firstDto.Salary);
			Assert.Equal(firstEmployee.SuggestedBonus, firstDto.SuggestedBonus);
			Assert.Equal(firstEmployee.YearsInService, firstDto.YearsInService);
		}
	}
}
