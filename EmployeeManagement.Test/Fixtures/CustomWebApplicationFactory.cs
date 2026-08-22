using System;
using System.Collections.Generic;
using System.Text;
using EmployeeManagement.DataAccess.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Test.Fixtures
{
	public class CustomWebApplicationFactory : WebApplicationFactory<Program>
	{
		private SqliteConnection? _connection;
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				var descriptors = services.Where(d => d.ServiceType == typeof(DbContextOptions<EmployeeDbContext>)
					|| d.ServiceType == typeof(EmployeeDbContext)
				).ToList();
				
				foreach (var descriptor in descriptors)
				{
					services.Remove(descriptor);
				}

				_connection = new SqliteConnection("DataSource=:memory:");
				_connection.Open();

				services.AddDbContext<EmployeeDbContext>(options =>
				{
					options.UseSqlite(_connection);
				});

				var sp = services.BuildServiceProvider();
				using var scope = sp.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
				db.Database.EnsureCreated();
			});
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_connection?.Dispose();
		}
	}
}
