using ForumFuncionario.Api.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ForumFuncionario.Api.Controllers
{
    /// <summary>
    /// Classe base para todos os controladores, fornecendo métodos de resposta comuns.
    /// </summary>
    public abstract class BaseController(ILogger<BaseController> logger) : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger = logger;

        /// <summary>
        /// Cria uma resposta paginada com links de navegação.
        /// </summary>
        /// <typeparam name="T">Tipo dos dados.</typeparam>
        /// <param name="data">Lista de dados a serem retornados.</param>
        /// <param name="meta">Metadados da resposta.</param>
        /// <param name="actionName">Nome da ação.</param>
        /// <param name="routeValues">Valores das rotas.</param>
        /// <returns>Uma resposta com os dados e metadados.</returns>
        protected IActionResult CreateResponse<T>(List<T> data, MetaData meta, string actionName, object routeValues) where T : class
        {
            var links = new List<LinkInfo>();

            var selfUrl = Url.Action(actionName, routeValues);
            if (!string.IsNullOrEmpty(selfUrl))
            {
                links.Add(new LinkInfo("self", selfUrl, "GET"));
            }

            if (meta.CurrentPage < meta.TotalPages)
            {
                var nextUrl = Url.Action(actionName, new { pageNumber = meta.CurrentPage + 1, pageSize = meta.ItemsPerPage });
                if (!string.IsNullOrEmpty(nextUrl))
                {
                    links.Add(new LinkInfo("next", nextUrl, "GET"));
                }
            }

            if (meta.CurrentPage > 1)
            {
                var previousUrl = Url.Action(actionName, new { pageNumber = meta.CurrentPage - 1, pageSize = meta.ItemsPerPage });
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    links.Add(new LinkInfo("previous", previousUrl, "GET"));
                }
            }

            var response = new BaseResponse<List<T>>
            {
                Data = data,
                Meta = meta,
                Links = links
            };

            return Ok(response);
        }

        /// <summary>
        /// Cria uma resposta para um único objeto.
        /// </summary>
        /// <typeparam name="T">Tipo dos dados.</typeparam>
        /// <param name="data">Dados a serem retornados.</param>
        /// <param name="actionName">Nome da ação.</param>
        /// <param name="routeValues">Valores das rotas.</param>
        /// <returns>Uma resposta com os dados.</returns>
        protected IActionResult CreateResponse<T>(T data, string actionName, object routeValues)
        {
            var links = new List<LinkInfo>();

            var selfUrl = Url.Action(actionName, routeValues);
            if (!string.IsNullOrEmpty(selfUrl))
            {
                links.Add(new LinkInfo("self", selfUrl, "GET"));
            }

            var response = new BaseResponse<T>
            {
                Data = data,
                Links = links
            };

            return Ok(response);
        }

        /// <summary>
        /// Trata um erro 404 (Não Encontrado) e registra a mensagem de erro.
        /// </summary>
        /// <typeparam name="T">Tipo dos dados.</typeparam>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta NotFound com a mensagem de erro.</returns>
        protected IActionResult HandleNotFound<T>(string message) where T : class
        {
            _logger.LogError(message);
            return NotFound(new BaseResponse<List<T>> { Error = new ErrorResponse(404) { Message = message } });
        }

        /// <summary>
        /// Trata um erro 400 (Solicitação Inválida) e registra os erros de validação.
        /// </summary>
        /// <param name="modelState">Estado do modelo contendo erros de validação.</param>
        /// <returns>Resposta BadRequest com os detalhes dos erros.</returns>
        protected IActionResult HandleBadRequest(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            _logger.LogWarning("Bad request: {errors}", string.Join("; ", errors));

            return BadRequest(new BaseResponse<bool>
            {
                Error = new ErrorResponse(400) { Message = "A solicitação contém dados inválidos.", Details = errors }
            });
        }

        /// <summary>
        /// Trata um erro 400 (Solicitação Inválida) e registra uma mensagem de erro.
        /// </summary>
        /// <param name="errorMessage">Mensagem de erro a ser registrada.</param>
        /// <returns>Resposta BadRequest com a mensagem de erro.</returns>
        protected IActionResult HandleBadRequest(string errorMessage)
        {
            _logger.LogWarning("Bad request: {errorMessage}", errorMessage);

            return BadRequest(new BaseResponse<bool>
            {
                Error = new ErrorResponse(400) { Message = "A solicitação contém dados inválidos.", Details = new List<string> { errorMessage } }
            });
        }


        /// <summary>
        /// Trata um erro 500 (Erro Interno do Servidor) e registra a mensagem de erro.
        /// </summary>
        /// <typeparam name="T">Tipo dos dados.</typeparam>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta com erro interno do servidor.</returns>
        protected IActionResult HandleServerError<T>(string message) where T : class
        {
            _logger.LogError(message);
            return StatusCode(500, new BaseResponse<T> { Error = new ErrorResponse(500) { Message = message } });
        }

        /// <summary>
        /// Trata um erro 500 (Erro Interno do Servidor) e registra a mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta com erro interno do servidor.</returns>
        protected IActionResult HandleServerError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, new BaseResponse<bool> { Error = new ErrorResponse(500) { Message = message } });
        }

        /// <summary>
        /// Trata um erro 401 (Não Autorizado) e registra a mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        /// <returns>Resposta Unauthorized com a mensagem de erro.</returns>
        protected IActionResult HandleUnauthorized(string message)
        {
            _logger.LogWarning("Unauthorized: {message}", message);
            return Unauthorized(new BaseResponse<bool>
            {
                Error = new ErrorResponse(401) { Message = message }
            });
        }

    }
}
