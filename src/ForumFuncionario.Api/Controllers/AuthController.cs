using ForumFuncionario.Api.Model.Enum;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : BaseController(logger)
    {

        /// <summary>
        /// Verifica se o cadastro de um usuário existe.
        /// </summary>
        /// <param name="usuario">Nome de usuário a ser verificado.</param>
        /// <returns>Uma resposta indicando o status do cadastro e uma mensagem correspondente.</returns>
        [AllowAnonymous]
        [HttpGet("VerificarCadastro/{usuario}")]
        [ProducesResponseType(typeof(BaseResponse<VerificarCadastroResponse>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]     // Não Encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]     // Erro Interno do Servidor
        public async Task<IActionResult> VerificarCadastro(string usuario)
        {
            try
            {
                // Verifica se o usuário existe na tabela de funcionários
                var userProtheus = await authService.GetUserProtheusByUsername(usuario);

                if (userProtheus == null)
                {
                    // Retorna que o usuário não foi encontrado
                    return CreateResponse(new VerificarCadastroResponse
                    {
                        Status = CadastroStatus.USUARIO_NAO_ENCONTRADO.ToString(),
                        Mensagem = "Usuário não encontrado."
                    }, nameof(VerificarCadastro), new { usuario });
                }

                // Verifica se o usuário já está cadastrado no sistema
                var usuarioCadastrado = await authService.GetUserAppByUsername(usuario);

                if (usuarioCadastrado == null)
                {
                    // Retorna que o usuário não está cadastrado
                    return CreateResponse(new VerificarCadastroResponse
                    {
                        Status = CadastroStatus.USUARIO_NAO_REGISTRADO.ToString(),
                        Mensagem = "Usuário encontrado, mas não registrado."
                    }, nameof(VerificarCadastro), new { usuario });
                }

                // Retorna que o usuário está cadastrado
                return CreateResponse(new VerificarCadastroResponse
                {
                    Login = true,
                    Status = CadastroStatus.USUARIO_REGISTRADO.ToString(),
                    Mensagem = "Usuário registrado."
                }, nameof(VerificarCadastro), new { usuario });
            }
            catch (Exception ex)
            {
                // Log do erro e retorno de erro genérico
                logger.LogError(ex, "Erro ao verificar cadastro para o usuário {Usuario}", usuario);
                return HandleServerError($"Ocorreu um erro ao verificar o cadastro do usuário: {usuario}.");
            }
        }

        /// <summary>
        /// Registra um novo usuário.
        /// </summary>
        /// <param name="signUpDto">Dados do novo usuário a ser registrado.</param>
        /// <returns>Uma resposta indicando o sucesso ou falha do registro do usuário.</returns>
        [AllowAnonymous]
        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 500)] // Erro Interno do Servidor
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpDto)
        {
            try
            {
                logger.LogInformation("Tentando registrar o usuário...");
                var response = await authService.SignUp(signUpDto);
                if (response)
                {
                    logger.LogInformation("Usuário registrado com sucesso.");
                    return CreateResponse(response, nameof(SignUp), null);
                }
                else
                {
                    return HandleServerError("Falha ao registrar o usuário.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao registrar o usuário.");
                return HandleServerError("Um erro inesperado ocorreu durante o registro do usuário.");
            }
        }
        /// <summary>
        /// Faz login de um usuário.
        /// </summary>
        /// <param name="signInDTO">Dados do login do usuário, incluindo nome de usuário e senha.</param>
        /// <returns>Uma resposta com os dados do usuário logado ou mensagem de erro.</returns>
        [AllowAnonymous]
        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(BaseResponse<SsoResponse>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]        // Não Encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]        // Erro Interno do Servidor
        public async Task<IActionResult> SignIn([FromBody] SignInRequest signInDTO)
        {
            try
            {
                // Obtendo o IP do cliente
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP não disponível";
                logger.LogInformation("Tentando fazer login do usuário a partir do IP: {ClientIp}", clientIp);

                var response = await authService.SignIn(signInDTO, clientIp);
                if (response != null)
                {
                    logger.LogInformation("Usuário logado com sucesso a partir do IP: {ClientIp}", clientIp);
                    return CreateResponse(response, nameof(SignIn), null);
                }
                else
                {
                    return HandleNotFound<SsoResponse>("Falha ao fazer login do usuário.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao fazer login do usuário.");
                return HandleServerError("Um erro inesperado ocorreu durante o login do usuário.");
            }
        }


        /// <summary>
        /// Adiciona um usuário ao papel de admin.
        /// </summary>
        /// <param name="userId">ID do usuário a ser adicionado ao papel de admin.</param>
        /// <returns>Uma resposta indicando o sucesso ou falha da operação.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("add-user-to-admin-role")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 500)] // Erro Interno do Servidor
        public async Task<IActionResult> AddUserToAdminRole([FromBody] int userId)
        {
            try
            {
                logger.LogInformation($"Tentando adicionar o usuário {userId} ao papel de admin...");
                await authService.AddUserToAdminRole(userId);
                logger.LogInformation($"Usuário {userId} adicionado ao papel de admin com sucesso.");
                return CreateResponse<bool>(true, nameof(AddUserToAdminRole), new { userId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ocorreu um erro ao adicionar o usuário {userId} ao papel de admin.");
                return HandleServerError($"Um erro inesperado ocorreu ao adicionar o usuário {userId} ao papel de admin.");
            }
        }

        /// <summary>
        /// Recupera o usuário atual.
        /// </summary>
        /// <returns>Uma resposta com os dados do usuário atual ou mensagem de erro.</returns>
        [HttpGet("get-current-user")]
        [ProducesResponseType(typeof(BaseResponse<UserApp>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]    // Não Encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]    // Erro Interno do Servidor
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                logger.LogInformation("Tentando recuperar o usuário atual...");
                var response = await authService.GetCurrentUser();
                if (response != null)
                {
                    logger.LogInformation("Usuário atual recuperado com sucesso.");
                    return CreateResponse(response, nameof(GetCurrentUser), null);
                }
                else
                {
                    return HandleNotFound<UserApp>("Falha ao recuperar o usuário atual.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao recuperar o usuário atual.");
                return HandleServerError("Um erro inesperado ocorreu ao recuperar o usuário atual.");
            }
        }

        /// <summary>
        /// Lista todos os usuários.
        /// </summary>
        /// <returns>Uma lista de usuários registrados no sistema.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("list-users")]
        [ProducesResponseType(typeof(BaseResponse<List<UserApp>>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]          // Não Encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]          // Erro Interno do Servidor
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                logger.LogInformation("Tentando listar usuários...");
                var response = await authService.ListUsers();
                if (response != null)
                {
                    logger.LogInformation("Usuários listados com sucesso.");
                    return CreateResponse(response, new MetaData(response.Count, response.Count, 1, 1), nameof(ListUsers), null);
                }
                else
                {
                    return HandleNotFound<UserApp>("Falha ao listar usuários.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao listar usuários.");
                return HandleServerError("Um erro inesperado ocorreu ao listar usuários.");
            }
        }

        /// <summary>
        /// Recupera um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário a ser recuperado.</param>
        /// <returns>Uma resposta com os dados do usuário ou mensagem de erro.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponse<UserApp>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]    // Não Encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]    // Erro Interno do Servidor
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                logger.LogInformation($"Tentando recuperar o usuário com ID {id}...");
                var response = await authService.GetUserById(id);
                if (response != null)
                {
                    logger.LogInformation($"Usuário com ID {id} recuperado com sucesso.");
                    return CreateResponse(response, nameof(GetUser), null);
                }
                else
                {
                    return HandleNotFound<UserApp>("Usuário não encontrado.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao recuperar o usuário.");
                return HandleServerError("Um erro inesperado ocorreu ao recuperar o usuário.");
            }
        }
    }
}
