using System;
using System.Collections.Generic;
using System.Text;
using EmployeeManagement.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace EmployeeManagement.Test
{
	public class CheckShowStaticsHeaderTests
	{
		[Fact]
		public void OnActionExecuting_HeaderMissing_MustReturnBadRequest()
		{
			// Arrange
			var actionFilter = new CheckShowStatisticsHeader();
			var httpContext = new DefaultHttpContext();

			var actionContext = new ActionContext(
				httpContext,
				new RouteData(),
				new ActionDescriptor());

			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				new Dictionary<string, object?>(),
				controller: null!);

			// Act
			actionFilter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.IsType<BadRequestResult>(actionExecutingContext.Result);
		}

		[Fact]
		public void OnActionExecuting_HeaderPresentAndTrue_MustNotSetResult()
		{
			// Arrange
			var actionFilter = new CheckShowStatisticsHeader();
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers["ShowStatistics"] = "true";

			var actionContext = new ActionContext(
				httpContext,
				new RouteData(),
				new ActionDescriptor());

			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				new Dictionary<string, object?>(),
				controller: null!);

			// Act
			actionFilter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.Null(actionExecutingContext.Result);
		}

		[Fact]
		public void OnActionExecuting_HeaderPresentButNotBoolean_MustReturnBadRequest()
		{
			// Arrange
			var actionFilter = new CheckShowStatisticsHeader();
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers["ShowStatistics"] = "maybe";

			var actionContext = new ActionContext(
				httpContext,
				new RouteData(),
				new ActionDescriptor());

			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				new Dictionary<string, object?>(),
				controller: null!);

			// Act
			actionFilter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.IsType<BadRequestResult>(actionExecutingContext.Result);
		}

		[Fact]
		public void OnActionExecuting_HeaderPresentButFalse_MustReturnBadRequest()
		{
			// Arrange
			var actionFilter = new CheckShowStatisticsHeader();
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers["ShowStatistics"] = "false";

			var actionContext = new ActionContext(
				httpContext,
				new RouteData(),
				new ActionDescriptor());

			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				new Dictionary<string, object?>(),
				controller: null!);

			// Act
			actionFilter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.IsType<BadRequestResult>(actionExecutingContext.Result);
		}
	}
}
