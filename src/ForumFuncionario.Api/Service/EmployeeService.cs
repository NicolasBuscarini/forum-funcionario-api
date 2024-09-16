using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;

namespace ForumFuncionario.Api.Service
{
    public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
    {
        public Task<IEnumerable<Employee>> GetEmployeesByMonthAsync()
        {
            return employeeRepository.GetEmployeesByMonthAsync();
        }
    }
}