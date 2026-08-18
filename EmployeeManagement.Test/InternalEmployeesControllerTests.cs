using System;
using System.Collections.Generic;
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

			var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeProfile>, new LoggerFactory());
			var mapper = new Mapper(mapperConfiguration);

			var controller = new InternalEmployeesController(employeeServiceMock.Object, mapper);

			// Act
			var result = await controller.GetInternalEmployees();

			// Assert
			var actionResult = Assert.IsType<ActionResult<IEnumerable<InternalEmployeeDto>>>(result);
			var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
			var dtos = Assert.IsAssignableFrom<IEnumerable<InternalEmployeeDto>>(okObjectResult.Value);
			Assert.Equal(2, dtos.Count());)



		}
	}
}
