using ForumFuncionario.Api.Model.Entity;

namespace ForumFuncionario.Api.Repository.Interface
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesByMonthAsync();
    }
}