using EmployeeManagement.Business;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;
using Moq;

namespace EmployeeManagement.Test.Fixtures
{
	public class EmployeeServiceFixture : IDisposable
	{
		public EmployeeService EmployeeService { get; }
		public Mock<IEmployeeManagementRepository> EmployeeManagementRepository { get; }
		public EmployeeServiceFixture()
		{
			EmployeeManagementRepository = new Mock<IEmployeeManagementRepository>();

			EmployeeManagementRepository
				.Setup(m => m.GetInternalEmployeeAsync(It.IsAny<Guid>()))
				.ReturnsAsync(new InternalEmployee("Tony", "Hall", 2, 2500, false, 2)
				{
					AttendedCourses = new List<Course>
					{
						new Course("A course"),
						new Course("Another course")
					}
				});

			EmployeeManagementRepository
				.Setup(m => m.SaveChangesAsync())
				.Returns(Task.CompletedTask);

			EmployeeService = new EmployeeService(EmployeeManagementRepository.Object, new EmployeeFactory());
		}

		public void Dispose()
		{
			// clean up the setup code, if required
		}
	}
}
