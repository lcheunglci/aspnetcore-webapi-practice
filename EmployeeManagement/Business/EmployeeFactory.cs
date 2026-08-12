using EmployeeManagement.DataAccess.Entities;

namespace EmployeeManagement.Business;

/// <summary>
/// Factory for creating employees
/// </summary>
public class EmployeeFactory
{
    /// <summary>
    /// Create an employee
    /// </summary>
    public virtual Employee CreateEmployee(string firstName,
        string lastName,
        string? company = null,
        bool isExternal = false)
    {
        if (string.IsNullOrEmpty(firstName))
        {
            throw new ArgumentException(
                $"'{nameof(firstName)}' cannot be null or empty.",
                nameof(firstName));
        }

        if (string.IsNullOrEmpty(lastName))
        {
            throw new ArgumentException(
                $"'{nameof(lastName)}' cannot be null or empty.",
                nameof(lastName));
        }

        if (company == null && isExternal)
        {
            throw new ArgumentException(
                $"'{nameof(company)}' cannot be null when the employee is external.",
                nameof(company));
        }

        if (isExternal)
        {
            return new ExternalEmployee(firstName, lastName, company!);
        }

        return new InternalEmployee(firstName, lastName, 0, 2500, false, 1);
    }
}
