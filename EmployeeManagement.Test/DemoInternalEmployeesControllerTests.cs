using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.MapperProfiles;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Test
{
	public class DemoInternalEmployeesControllerTests
	{
		[Fact]
		public async Task CreateInternalEmployee_InvalidModel_MustReturnBadRequest()
		{
			// Arrange
			var employeeServiceMock = new Mock<IEmployeeService>();
			var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeProfile>(), new LoggerFactory());

			var mapper = new Mapper(mapperConfiguration);

			var controller = new DemoInternalEmployeesController(employeeServiceMock.Object, mapper);

			// Act
			var result = await controller.CreateInternalEmployee(new InternalEmployeeForCreationDto());

			// Assert
			var actionResult = Assert.IsType<ActionResult<InternalEmployeeDto>>(result);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}
	}
}
