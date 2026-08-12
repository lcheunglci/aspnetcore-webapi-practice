using AutoMapper;
using EmployeeManagement.Business;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

[Route("api/protectedinternalemployees")]
[ApiController]
[Authorize(Policy = "MustBeAdmin")]
public class ProtectedInternalEmployeesController(
    IEmployeeService employeeService,
    IMapper mapper) : ControllerBase
{
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InternalEmployeeDto>>> GetInternalEmployees()
    {
        var internalEmployees = await _employeeService.FetchInternalEmployeesAsync();

        var internalEmployeeDtos =
            _mapper.Map<IEnumerable<InternalEmployeeDto>>(internalEmployees);

        return Ok(internalEmployeeDtos);
    }
}
