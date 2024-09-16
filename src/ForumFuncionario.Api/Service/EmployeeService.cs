using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public Task<IEnumerable<Employee>> GetEmployeesByMonthAsync()
    {
        return _employeeRepository.GetEmployeesByMonthAsync();
    }
}
