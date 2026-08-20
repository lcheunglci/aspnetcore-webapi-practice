using System.Security.Claims;
using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.MapperProfiles;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
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

		[Fact]
		public async Task GetProtectedInternalEmployee_UserIsAdmin_MustRedirectToProtectedController()
		{
			// Arrange
			var employeeServiceMock = new Mock<IEmployeeService>();
			var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeProfile>(), new LoggerFactory());

			var mapper = new Mapper(mapperConfiguration);

			var controller = new DemoInternalEmployeesController(employeeServiceMock.Object, mapper);

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
			{
				new Claim(ClaimTypes.Name, "Kevin"),
				new Claim(ClaimTypes.Role, "Admin")
			}, "TestAuthentication"));

			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = claimsPrincipal
				}
			};

			// Act
			var result = controller.GetProtectedInternalEmployees();

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("GetInternalEmployees", redirectResult.ActionName);
			Assert.Equal("ProtectedInternalEmployees", redirectResult.ControllerName);
		}


		[Fact]
		public async Task GetProtectedInternalEmployee_UserIsNotAdmin_MustRedirectToInternalEmployees()
		{
			// Arrange
			var employeeServiceMock = new Mock<IEmployeeService>();
			var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeProfile>(), new LoggerFactory());

			var mapper = new Mapper(mapperConfiguration);

			var controller = new DemoInternalEmployeesController(employeeServiceMock.Object, mapper);

			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
			{
				new Claim(ClaimTypes.Name, "Kevin"),
				new Claim(ClaimTypes.Role, "User")
			}, "TestAuthentication"));

			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext()
				{
					User = claimsPrincipal
				}
			};

			// Act
			var result = controller.GetProtectedInternalEmployees();

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("GetInternalEmployees", redirectResult.ActionName);
			Assert.Equal("InternalEmployees", redirectResult.ControllerName);
		}
	}
}
