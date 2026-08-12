using EmployeeManagement.DataAccess.DbContexts;
using EmployeeManagement.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DataAccess.Services;

public class EmployeeManagementRepository(EmployeeDbContext context) : IEmployeeManagementRepository
{
    private readonly EmployeeDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IEnumerable<InternalEmployee>> GetInternalEmployeesAsync()
    {
        return await _context.InternalEmployees
            .Include(e => e.AttendedCourses)
            .ToListAsync();
    }

    public async Task<InternalEmployee?> GetInternalEmployeeAsync(Guid employeeId)
    {
        return await _context.InternalEmployees
            .Include(e => e.AttendedCourses)
            .FirstOrDefaultAsync(e => e.Id == employeeId);
    }

    public async Task<Course?> GetCourseAsync(Guid courseId)
    {
        return await _context.Courses.FirstOrDefaultAsync(e => e.Id == courseId);
    }

    public async Task<IEnumerable<Course>> GetCoursesAsync()
    {
        return await _context.Courses.ToListAsync();
    }

    public async Task<List<Course>> GetCoursesAsync(params Guid[] courseIds)
    {
        List<Course> coursesToReturn = [];
        foreach (var courseId in courseIds)
        {
            var course = await GetCourseAsync(courseId);
            if (course != null)
            {
                coursesToReturn.Add(course);
            }
        }
        return coursesToReturn;
    }

    public void AddInternalEmployee(InternalEmployee internalEmployee)
    {
        _context.InternalEmployees.Add(internalEmployee);
    }

    public void AddCourse(Course course)
    {
        _context.Courses.Add(course);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
