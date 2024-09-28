using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Response;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger) : BaseController(logger)
    {
        /// <summary>
        /// Recupera a lista de funcionários para o mês atual.
        /// </summary>
        /// <remarks>
        /// Este método retorna todos os funcionários associados ao mês atual. Como não há paginação, a lista completa será retornada em uma única resposta, junto com os metadados.
        /// </remarks>
        /// <returns>
        /// Retorna uma lista de funcionários para o mês atual, juntamente com os metadados como total de itens, itens por página (sem paginação), e número total de páginas.
        /// </returns>
        /// <response code="200">Funcionários encontrados com sucesso.</response>
        /// <response code="404">Nenhum funcionário encontrado para o mês atual.</response>
        /// <response code="500">Erro interno do servidor ao tentar buscar os funcionários.</response>
        [HttpGet("current-month")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Employee>>), 200)]
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]
        public async Task<IActionResult> GetEmployeesByMonth()
        {
            _logger.LogInformation("Buscando funcionários para o mês atual.");

            try
            {
                var employees = await employeeService.GetEmployeesByMonthAsync();

                if (employees == null || !employees.Any())
                {
                    _logger.LogInformation("Nenhum funcionário encontrado para o mês atual.");
                    return HandleNotFound<Employee>("Nenhum funcionário encontrado para o mês atual.");
                }

                // Construindo os metadados
                var metaData = new MetaData(
                    totalItems: employees.Count(),
                    itemsPerPage: employees.Count(),  // Sem paginação, itens por página igual ao total de itens
                    currentPage: 1,                   // Única página, já que não há paginação
                    totalPages: 1                     // Total de páginas é 1
                );

                _logger.LogInformation("Funcionários recuperados com sucesso para o mês atual.");
                return CreateResponse(employees.ToList(), metaData, nameof(GetEmployeesByMonth), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao recuperar os funcionários.");
                return HandleServerError("Ocorreu um erro interno no servidor.");
            }
        }
    }
}
