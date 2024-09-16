using ForumFuncionario.Api.Model.Entity;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetEmployeesByMonthAsync();
}

