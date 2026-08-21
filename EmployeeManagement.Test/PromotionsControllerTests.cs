using System;
using System.Collections.Generic;
using System.Text;
using EmployeeManagement.Business;
using EmployeeManagement.Controllers;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using EmployeeManagement.Models;
using EmployeeManagement.Test.HttpMessageHandlers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EmployeeManagement.Test
{
	public class PromotionsControllerTests
	{
		[Fact]
		public async Task CreatePromotion_EmployeeIsEligible_MustReturnOkWithIncrementedJobLevel()
		{
			// Arrange
			Guid employeeId = Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb");

			var internalEmployee = new InternalEmployee("Megan", "Jones", 2, 3000, false, 2)
			{
				Id = employeeId,
				AttendedCourses = new List<Course>()
				{
					new Course("A course"),
					new Course("Another course")
				}
			};

			var employeeServiceMock = new Mock<IEmployeeService>();

			employeeServiceMock.Setup(m => m.FetchInternalEmployeeAsync(employeeId))
				.ReturnsAsync(internalEmployee);

			var httpClient = new HttpClient(
				new TestablePromotionEligibilityHandler(true));
			var employeeManagementRepositoryMock = new Mock<IEmployeeManagementRepository>();
			employeeManagementRepositoryMock.Setup(m => m.SaveChangesAsync())
				.Returns(Task.CompletedTask);

			var promotionService = new PromotionService(
				httpClient, employeeManagementRepositoryMock.Object);

			var controller = new PromotionsController(employeeServiceMock.Object, promotionService);

			// Act
			var result = await controller.CreatePromotion(new PromotionForCreationDto() { EmployeeId = employeeId });

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var promotionResult = Assert.IsType<PromotionResultDto>(okResult.Value);
			Assert.Equal(employeeId, promotionResult.EmployeeId);
			Assert.Equal(3, promotionResult.JobLevel);

		}

		[Fact]
		public async Task CreatePromotion_EmployeeNotIsEligible_MustReturnBadRequest()
		{
			// Arrange
			Guid employeeId = Guid.Parse("72f2f5fe-e50c-4966-8420-d50258aefdcb");

			var internalEmployee = new InternalEmployee("Megan", "Jones", 2, 3000, false, 2)
			{
				Id = employeeId,
				AttendedCourses = new List<Course>()
				{
					new Course("A course"),
					new Course("Another course")
				}
			};

			var employeeServiceMock = new Mock<IEmployeeService>();

			employeeServiceMock.Setup(m => m.FetchInternalEmployeeAsync(employeeId))
				.ReturnsAsync(internalEmployee);

			var httpClient = new HttpClient(
				new TestablePromotionEligibilityHandler(false));
			var employeeManagementRepositoryMock = new Mock<IEmployeeManagementRepository>();
			employeeManagementRepositoryMock.Setup(m => m.SaveChangesAsync())
				.Returns(Task.CompletedTask);

			var promotionService = new PromotionService(
				httpClient, employeeManagementRepositoryMock.Object);

			var controller = new PromotionsController(employeeServiceMock.Object, promotionService);

			// Act
			var result = await controller.CreatePromotion(new PromotionForCreationDto() { EmployeeId = employeeId });

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
		}
	}
}
