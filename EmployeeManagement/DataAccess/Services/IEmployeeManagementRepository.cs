using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.DataAccess.Services;

public interface IEmployeeManagementRepository
{
    Task<IEnumerable<InternalEmployee>> GetInternalEmployeesAsync();
    Task<InternalEmployee?> GetInternalEmployeeAsync(Guid employeeId);
    Task<IEnumerable<Course>> GetCoursesAsync();
    Task<Course?> GetCourseAsync(Guid courseId);
    Task<List<Course>> GetCoursesAsync(params Guid[] courseIds);
    void AddCourse(Course course);
    void AddInternalEmployee(InternalEmployee internalEmployee);
    Task SaveChangesAsync();
}
