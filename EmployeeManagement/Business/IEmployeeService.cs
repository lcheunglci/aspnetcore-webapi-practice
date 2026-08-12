using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Business;

public interface IEmployeeService
{
    Task AddInternalEmployeeAsync(InternalEmployee internalEmployee);
    Task<InternalEmployee> CreateInternalEmployeeAsync(string firstName,
        string lastName);
    Task<InternalEmployee?> FetchInternalEmployeeAsync(Guid employeeId);
    Task<IEnumerable<InternalEmployee>> FetchInternalEmployeesAsync();
    Task GiveMinimumRaiseAsync(InternalEmployee employee);
    Task GiveRaiseAsync(InternalEmployee employee, int raise);
}
