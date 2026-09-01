using System;
using System.Collections.Generic;
using System.Text;
using EmployeeManagement.Business;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Internal;

namespace EmployeeManagement.Test
{
	public class ServiceRegistrationTests
	{
		[Fact]
		public void RegisterBusinessServices_MustResolveEmplyeeService()
		{
			// Arrange
			var services = new ServiceCollection();
			services.RegisterBusinessServices();

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string?>
				{
					{ "ConnectionStrings:EmployeeManagementDB", "DataSource=:memory:" }
				})
				.Build();

			services.RegisterDataServices(configuration);

			var serviceProvider = services.BuildServiceProvider();

			// Act
			var employeeService = serviceProvider.GetService<IEmployeeService>();

			// Assert
			Assert.NotNull(employeeService);
			Assert.IsType<EmployeeService>(employeeService);
		}
	}
}
