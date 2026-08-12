using EmployeeManagement.Business.Exceptions;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.DataAccess.Services;

namespace EmployeeManagement.Business;

public class EmployeeService(IEmployeeManagementRepository repository,
    EmployeeFactory employeeFactory) : IEmployeeService
{
    private readonly Guid[] _obligatoryCourseIds = [
        Guid.Parse("37e03ca7-c730-4351-834c-b66f280cdb01"),
        Guid.Parse("1fd115cf-f44c-4982-86bc-a8fe2e4ff83e") ];

    private readonly IEmployeeManagementRepository _repository = repository;
    private readonly EmployeeFactory _employeeFactory = employeeFactory;

    public async Task GiveMinimumRaiseAsync(InternalEmployee employee)
    {
        employee.Salary += 100;
        employee.MinimumRaiseGiven = true;
        await _repository.SaveChangesAsync();
    }

    public async Task GiveRaiseAsync(InternalEmployee employee, int raise)
    {
        if (raise < 100)
        {
            throw new EmployeeInvalidRaiseException(
                "Invalid raise: raise must be higher than or equal to 100.", raise);
        }

        if (employee.MinimumRaiseGiven && raise == 100)
        {
            throw new EmployeeInvalidRaiseException(
                "Invalid raise: minimum raise cannot be given twice.", raise);
        }

        if (raise == 100)
        {
            await GiveMinimumRaiseAsync(employee);
        }
        else
        {
            employee.Salary += raise;
            employee.MinimumRaiseGiven = false;
            await _repository.SaveChangesAsync();
        }
    }

    public async Task<InternalEmployee?> FetchInternalEmployeeAsync(Guid employeeId)
    {
        var employee = await _repository.GetInternalEmployeeAsync(employeeId);

        if (employee != null)
        {
            employee.SuggestedBonus = CalculateSuggestedBonus(employee);
        }
        return employee;
    }

    public async Task<IEnumerable<InternalEmployee>> FetchInternalEmployeesAsync()
    {
        var employees = await _repository.GetInternalEmployeesAsync();

        foreach (var employee in employees)
        {
            employee.SuggestedBonus = CalculateSuggestedBonus(employee);
        }

        return employees;
    }

    public async Task<InternalEmployee> CreateInternalEmployeeAsync(
       string firstName, string lastName)
    {
        var employee = (InternalEmployee)_employeeFactory.CreateEmployee(
            firstName, lastName);

        var obligatoryCourses = await _repository.GetCoursesAsync(
            _obligatoryCourseIds);

        foreach (var obligatoryCourse in obligatoryCourses)
        {
            employee.AttendedCourses.Add(obligatoryCourse);
        }

        employee.SuggestedBonus = CalculateSuggestedBonus(employee);
        return employee;
    }

    public async Task AddInternalEmployeeAsync(InternalEmployee internalEmployee)
    {
        _repository.AddInternalEmployee(internalEmployee);
        await _repository.SaveChangesAsync();
    }

    private int CalculateSuggestedBonus(InternalEmployee employee)
    {
        if (employee.YearsInService == 0)
        {
            return employee.AttendedCourses.Count * 100;
        }
        else
        {
            return employee.YearsInService
                * employee.AttendedCourses.Count * 100;
        }
    }
}
