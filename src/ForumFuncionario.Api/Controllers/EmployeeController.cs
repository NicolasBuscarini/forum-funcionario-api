using ForumFuncionario.Api.Model.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger) : ControllerBase
    {
        [HttpGet("current-month")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByMonth()
        {
            try
            {
                logger.LogInformation("Getting employees for the current month.");

                var employees = await employeeService.GetEmployeesByMonthAsync();

                if (employees == null || !employees.Any())
                {
                    logger.LogInformation("No employees found for the current month.");
                    return NotFound("No employees found.");
                }

                logger.LogInformation("Successfully retrieved employees for the current month.");
                return Ok(employees);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving employees.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
