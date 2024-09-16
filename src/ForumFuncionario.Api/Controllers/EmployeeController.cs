using ForumFuncionario.Api.Model.Entity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet("current-month")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByMonth()
    {
        var employees = await _employeeService.GetEmployeesByMonthAsync();
        return Ok(employees);
    }
}
