using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;
using ForumFuncionario.Api.Service;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RamalController(IRamalService RamalService, ILogger<RamalController> logger) : BaseController(logger)
    {
      

      
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Ramal>>), 200)]
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]
        public async Task<IActionResult> GetRamais()
        {
            try
            {
                var Ramals = await RamalService.GetRamal();

                if (Ramals == null || !Ramals.Any())
                {
                    _logger.LogInformation("Nenhum Ramal encontrado.");
                    return HandleNotFound<Ramal>("Nenhum Ramal encontrado.");
                }

                var metaData = new MetaData(
                    totalItems: Ramals.Count,
                    itemsPerPage: Ramals.Count,
                    currentPage: 1,
                    totalPages: 1
                );

                _logger.LogInformation("Ramals recuperados com sucesso.");
                return CreateResponse(Ramals, metaData, nameof(GetRamais), null);
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar Ramals.");
                return HandleServerError("Erro inesperado ao buscar Ramals. Por favor, tente novamente mais tarde.");
            }
        }

       
    }
}
