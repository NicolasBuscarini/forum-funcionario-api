using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController(IPostService postService, ILogger<PostController> logger) : BaseController(logger)
    {
        /// <summary>
        /// Cria um novo post com base nas informações fornecidas no corpo da requisição.
        /// </summary>
        /// <param name="request">Requisição contendo título, corpo, categoria e tags do post.</param>
        /// <returns>Uma ação que representa o resultado da criação do post.</returns>
        /// <response code="200">Post criado com sucesso.</response>
        /// <response code="400">Requisição inválida, problema com os dados fornecidos.</response>
        /// <response code="401">Usuário não autorizado para criar o post.</response>
        /// <response code="500">Erro inesperado ao criar o post.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<Post>), 200)]
        [ProducesResponseType(typeof(BaseResponse<string>), 400)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 401)]
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return HandleBadRequest(ModelState);
            }

            var username = User.Identity?.Name ?? "Anônimo";
            _logger.LogInformation($"Iniciando a criação de um post para o usuário {username}");

            try
            {
                var post = await postService.CreatePostAsync(request, username);

                if (post == null)
                {
                    _logger.LogError("Falha ao criar o post.");
                    return HandleServerError("Erro ao criar o post.");
                }

                _logger.LogInformation("Post criado com sucesso.");
                return CreateResponse(post, nameof(CreatePost), null);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Erro de validação na criação do post.");
                return HandleBadRequest(argEx.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return HandleUnauthorized("Acesso não autorizado ao criar o post.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado durante a criação do post.");
                return HandleServerError("Erro inesperado ao criar o post.");
            }
        }

        /// <summary>
        /// Obtém todos os posts de uma determinada categoria.
        /// </summary>
        /// <param name="categoria">Categoria para filtrar os posts.</param>
        /// <returns>Lista de posts da categoria especificada.</returns>
        /// <response code="200">Posts encontrados com sucesso.</response>
        /// <response code="404">Nenhum post encontrado para essa categoria.</response>
        /// <response code="500">Erro inesperado ao buscar posts.</response>
        [HttpGet("categoria/{categoria}")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Post>>), 200)]
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]
        public async Task<IActionResult> GetPostsByCategoria(string categoria)
        {
            _logger.LogInformation($"Buscando posts da categoria: {categoria}");

            try
            {
                var posts = await postService.GetPostsByCategoriaAsync(categoria);

                if (posts == null || !posts.Any())
                {
                    _logger.LogInformation("Nenhum post encontrado para esta categoria.");
                    return HandleNotFound<Post>("Nenhum post encontrado para a categoria informada.");
                }

                var metaData = new MetaData(
                    totalItems: posts.Count,
                    itemsPerPage: posts.Count,
                    currentPage: 1,
                    totalPages: 1
                );

                _logger.LogInformation("Posts recuperados com sucesso.");
                return CreateResponse(posts, metaData, nameof(GetPostsByCategoria), null);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação na categoria informada.");
                return HandleBadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts pela categoria.");
                return HandleServerError("Erro inesperado ao buscar posts por categoria. Por favor, tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Obtém os posts mais recentes de todas as categorias.
        /// </summary>
        /// <returns>Lista dos posts mais recentes.</returns>
        /// <response code="200">Posts recentes encontrados com sucesso.</response>
        /// <response code="404">Nenhum post recente encontrado.</response>
        /// <response code="500">Erro inesperado ao buscar posts recentes.</response>
        [HttpGet("recentes")]
        [Authorize]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<Post>>), 200)]
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]
        public async Task<IActionResult> GetLatestPostsByCategoria()
        {
            _logger.LogInformation("Buscando os posts mais recentes por categoria.");

            try
            {
                var latestPosts = await postService.GetLatestPostsByCategoriaAsync();

                if (latestPosts == null || !latestPosts.Any())
                {
                    _logger.LogInformation("Nenhum post recente encontrado.");
                    return HandleNotFound<Post>("Nenhum post recente encontrado.");
                }

                var metaData = new MetaData(
                    totalItems: latestPosts.Count,
                    itemsPerPage: latestPosts.Count,
                    currentPage: 1,
                    totalPages: 1
                );

                _logger.LogInformation("Posts recentes recuperados com sucesso.");
                return CreateResponse(latestPosts, metaData, nameof(GetLatestPostsByCategoria), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar posts recentes.");
                return HandleServerError("Erro inesperado ao buscar posts recentes.");
            }
        }
    }
}
