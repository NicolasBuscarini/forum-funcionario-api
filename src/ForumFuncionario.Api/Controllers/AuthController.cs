using ForumFuncionario.Api.Model.Enumerable;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : BaseController(logger)
    {

        /// <summary>
        /// Verifica o status de cadastro de um usuário no sistema.
        /// </summary>
        /// <param name="usuario">Nome de usuário a ser verificado.</param>
        /// <returns>
        /// Uma resposta indicando o status do cadastro do usuário:
        /// - `USUARIO_NAO_ENCONTRADO`: Se o usuário não for encontrado.
        /// - `USUARIO_NAO_REGISTRADO`: Se o usuário existir no Protheus, mas não estiver registrado no sistema.
        /// - `USUARIO_REGISTRADO`: Se o usuário estiver registrado no sistema.
        /// </returns>
        /// <response code="200">Status do cadastro verificado com sucesso.</response>
        /// <response code="404">Usuário não encontrado no Protheus.</response>
        /// <response code="500">Erro inesperado ao verificar o cadastro.</response>
        [AllowAnonymous]
        [HttpGet("VerificarCadastro/{usuario}")]
        [ProducesResponseType(typeof(BaseResponse<VerificarCadastroResponse>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]                      // Não Encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]                      // Erro Interno do Servidor
        public async Task<IActionResult> VerificarCadastro(string usuario)
        {
            try
            {
                logger.LogInformation("Verificando cadastro para o usuário {Usuario}...", usuario);

                // Verifica se o usuário existe na tabela de funcionários (Protheus)
                var userProtheus = await authService.GetUserProtheusByUsername(usuario);
                if (userProtheus == null)
                {
                    logger.LogWarning("Usuário {Usuario} não encontrado no Protheus.", usuario);
                    return CreateResponse(new VerificarCadastroResponse
                    {
                        Status = CadastroStatus.USUARIO_NAO_ENCONTRADO.ToString(),
                        Mensagem = "Usuário não encontrado."
                    }, nameof(VerificarCadastro), new { usuario });
                }

                // Verifica se o usuário já está registrado no sistema
                var usuarioCadastrado = await authService.GetUserAppByUsername(usuario);
                if (usuarioCadastrado == null)
                {
                    logger.LogInformation("Usuário {Usuario} encontrado no Protheus, mas não registrado no sistema.", usuario);
                    return CreateResponse(new VerificarCadastroResponse
                    {
                        Status = CadastroStatus.USUARIO_NAO_REGISTRADO.ToString(),
                        Mensagem = "Usuário encontrado, mas não registrado."
                    }, nameof(VerificarCadastro), new { usuario });
                }

                // Usuário está registrado
                logger.LogInformation("Usuário {Usuario} registrado no sistema.", usuario);
                return CreateResponse(new VerificarCadastroResponse
                {
                    Login = true,
                    Status = CadastroStatus.USUARIO_REGISTRADO.ToString(),
                    Mensagem = "Usuário registrado."
                }, nameof(VerificarCadastro), new { usuario });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao verificar cadastro para o usuário {Usuario}.", usuario);
                return HandleServerError($"Ocorreu um erro ao verificar o cadastro do usuário: {usuario}.");
            }
        }


        /// <summary>
        /// Registra um novo usuário no sistema.
        /// O processo de registro verifica se o usuário já está cadastrado na tabela de funcionários (Protheus) 
        /// e se o nome de usuário já existe no sistema.
        /// </summary>
        /// <param name="signUpDto">Dados do novo usuário a ser registrado, incluindo nome de usuário e senha.</param>
        /// <returns>
        /// Uma resposta indicando se o registro foi bem-sucedido ou uma mensagem de erro detalhada em caso de falha.
        /// </returns>
        /// <response code="200">Usuário registrado com sucesso.</response>
        /// <response code="400">Quando o nome de usuário já existe ou o usuário não está cadastrado na tabela de funcionários.</response>
        /// <response code="500">Erro inesperado no servidor durante o processo de registro.</response>
        [AllowAnonymous]
        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 400)] // Solicitação inválida (dados inconsistentes)
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
            catch (ArgumentException ex) when (ex.Message.Contains("Usuário não possui cadastro na tabela de funcionários"))
            {
                // Caso o usuário não esteja registrado no Protheus
                logger.LogWarning(ex, "Tentativa de registro falhou: usuário não registrado no Protheus.");
                ModelState.AddModelError("Username", "Usuário não encontrado na tabela de funcionários.");
                return HandleBadRequest(ModelState);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Nome de usuário já existe"))
            {
                // Caso o nome de usuário já exista
                logger.LogWarning(ex, "Tentativa de registro falhou: nome de usuário já existe.");
                ModelState.AddModelError("Username", "Nome de usuário já existe.");
                return HandleBadRequest(ModelState);
            }
            catch (ArgumentException ex)
            {
                // Qualquer outro erro de argumento
                logger.LogWarning(ex, "Erro de validação durante o registro do usuário.");
                ModelState.AddModelError("General", ex.Message);
                return HandleBadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Erro inesperado
                logger.LogError(ex, "Ocorreu um erro ao registrar o usuário.");
                return HandleServerError("Um erro inesperado ocorreu durante o registro do usuário.");
            }
        }


        /// <summary>
        /// Realiza o login de um usuário com base nas credenciais fornecidas. 
        /// Se o login for bem-sucedido, retorna um token JWT e os detalhes do usuário.
        /// </summary>
        /// <param name="signInDTO">Dados de login do usuário, incluindo nome de usuário e senha.</param>
        /// <returns>
        /// Uma resposta contendo os dados do usuário logado, incluindo o token JWT, ou uma mensagem de erro.
        /// </returns>
        /// <response code="200">Retorna os dados do usuário autenticado e o token JWT.</response>
        /// <response code="401">Retorna quando a senha fornecida é incorreta.</response>
        /// <response code="404">Retorna quando o usuário não é encontrado no sistema.</response>
        /// <response code="500">Erro inesperado no servidor durante o processo de login.</response>
        [AllowAnonymous]
        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(BaseResponse<SsoResponse>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 401)]        // Não Autorizado (senha incorreta)
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]        // Não Encontrado (usuário não encontrado)
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
            catch (ArgumentException ex) when (ex.Message.Contains("senha") || ex.Message.Contains("password"))
            {
                // Caso a senha seja inválida
                logger.LogWarning(ex, "Tentativa de login com senha inválida.");
                return HandleUnauthorized("Senha incorreta.");
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Usuário não encontrado"))
            {
                // Caso o usuário não seja encontrado
                logger.LogWarning(ex, "Usuário não encontrado durante o login.");
                return HandleNotFound<SsoResponse>("Usuário não encontrado.");
            }
            catch (Exception ex)
            {
                // Erro inesperado
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
        /// Recupera o usuário atualmente autenticado.
        /// Se nenhum usuário estiver autenticado, retorna uma mensagem de erro.
        /// </summary>
        /// <returns>
        /// Uma resposta com os dados do usuário autenticado ou uma mensagem de erro se o usuário não for encontrado.
        /// </returns>
        /// <response code="200">Usuário atual recuperado com sucesso.</response>
        /// <response code="404">Usuário atual não encontrado ou não autenticado.</response>
        /// <response code="500">Erro inesperado no servidor ao tentar recuperar o usuário.</response>
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
                    logger.LogWarning("Nenhum usuário autenticado encontrado.");
                    return HandleNotFound<UserApp>("Nenhum usuário autenticado encontrado.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao recuperar o usuário atual.");
                return HandleServerError("Um erro inesperado ocorreu ao recuperar o usuário atual.");
            }
        }


        /// <summary>
        /// Lista todos os usuários registrados no sistema.
        /// O método é protegido, permitindo acesso apenas a usuários com o papel de "Admin".
        /// Retorna uma lista com os usuários encontrados ou uma mensagem de erro se não houver usuários.
        /// </summary>
        /// <returns>
        /// Uma resposta contendo a lista de usuários registrados ou uma mensagem de erro.
        /// </returns>
        /// <response code="200">Usuários listados com sucesso.</response>
        /// <response code="404">Nenhum usuário encontrado.</response>
        /// <response code="500">Erro inesperado no servidor ao tentar listar os usuários.</response>
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
                if (response != null && response.Any())
                {
                    logger.LogInformation("Usuários listados com sucesso.");
                    return CreateResponse(response, new MetaData(response.Count, response.Count, 1, 1), nameof(ListUsers), null);
                }
                else
                {
                    logger.LogWarning("Nenhum usuário encontrado.");
                    return HandleNotFound<UserApp>("Nenhum usuário encontrado.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao listar usuários.");
                return HandleServerError("Um erro inesperado ocorreu ao listar usuários.");
            }
        }


        /// <summary>
        /// Recupera um usuário pelo ID fornecido.
        /// O método tenta localizar o usuário pelo ID na base de dados. Se o usuário for encontrado,
        /// os dados do usuário são retornados; caso contrário, uma mensagem de erro é exibida.
        /// </summary>
        /// <param name="id">ID do usuário a ser recuperado.</param>
        /// <returns>
        /// Uma resposta com os dados do usuário, ou uma mensagem de erro se o usuário não for encontrado ou ocorrer um erro no servidor.
        /// </returns>
        /// <response code="200">Usuário recuperado com sucesso.</response>
        /// <response code="404">Usuário não encontrado para o ID fornecido.</response>
        /// <response code="500">Erro inesperado no servidor ao tentar recuperar o usuário.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponse<UserApp>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]    // Não Encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]    // Erro Interno do Servidor
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                logger.LogInformation("Tentando recuperar o usuário com ID {UserId}...", id);

                var response = await authService.GetUserById(id);
                if (response != null)
                {
                    logger.LogInformation("Usuário com ID {UserId} recuperado com sucesso.", id);
                    return CreateResponse(response, nameof(GetUser), null);
                }
                else
                {
                    return HandleNotFound<UserApp>("Usuário não encontrado.");
                }
            }
            catch (ArgumentException ex) when (ex.Message.Contains("User does not exist"))
            {
                // Tratamento para quando o usuário não for encontrado
                logger.LogWarning(ex, "Usuário com ID {UserId} não encontrado.", id);
                return HandleNotFound<UserApp>("Usuário não encontrado.");
            }
            catch (Exception ex)
            {
                // Tratamento de erro inesperado
                logger.LogError(ex, "Ocorreu um erro ao tentar recuperar o usuário com ID {UserId}.", id);
                return HandleServerError("Um erro inesperado ocorreu ao recuperar o usuário.");
            }
        }

        /// <summary>
        /// Inicia o processo de recuperação de senha.
        /// Envia um e-mail com um token para redefinir a senha do usuário.
        /// </summary>
        /// <param name="forgotPasswordRequest">Dados de solicitação de recuperação de senha, incluindo o nome de usuário ou e-mail.</param>
        /// <returns>
        /// Uma resposta indicando o sucesso da solicitação ou uma mensagem de erro.
        /// </returns>
        /// <response code="200">Solicitação de recuperação de senha processada com sucesso.</response>
        /// <response code="404">Usuário não encontrado.</response>
        /// <response code="500">Erro inesperado no servidor.</response>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]   // Sucesso
        [ProducesResponseType(typeof(BaseResponse<string>), 404)] // Usuário não encontrado
        [ProducesResponseType(typeof(BaseResponse<string>), 500)] // Erro Interno do Servidor
        public async Task<IActionResult> ForgotPassword([FromBody] Model.Request.ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {
                logger.LogInformation("Processando solicitação de recuperação de senha para o usuário {Email}", forgotPasswordRequest.Username);

                var response = await authService.EsqueciSenha(forgotPasswordRequest.Username);
                if (response)
                {
                    logger.LogInformation("E-mail de recuperação de senha enviado com sucesso.");
                    return CreateResponse(true, nameof(ForgotPassword), null);
                }
                else
                {
                    logger.LogWarning("Usuário {Username} não encontrado.", forgotPasswordRequest.Username);
                    return HandleNotFound<string>("Usuário não encontrado.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao processar a solicitação de recuperação de senha.");
                return HandleServerError("Um erro inesperado ocorreu ao processar a solicitação de recuperação de senha.");
            }
        }

        /// <summary>
        /// Redefine a senha do usuário usando o token de redefinição.
        /// </summary>
        /// <param name="resetPasswordRequest">Dados do token e nova senha.</param>
        /// <returns>
        /// Uma resposta indicando se a redefinição foi bem-sucedida.
        /// </returns>
        /// <response code="200">Senha redefinida com sucesso.</response>
        /// <response code="400">Token inválido ou dados inconsistentes.</response>
        /// <response code="500">Erro inesperado no servidor.</response>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]   // Sucesso (senha redefinida)
        [ProducesResponseType(typeof(BaseResponse<string>), 400)] // Token inválido ou erro nos dados
        [ProducesResponseType(typeof(BaseResponse<string>), 500)] // Erro Interno do Servidor
        public async Task<IActionResult> ResetPassword([FromBody] Model.Request.ResetPasswordRequest resetPasswordRequest)
        {
            try
            {
                logger.LogInformation("Processando redefinição de senha...");

                // Chama o serviço de autenticação para redefinir a senha
                var isSuccess = await authService.RedefinirSenhaAsync(
                    resetPasswordRequest.Username,
                    resetPasswordRequest.Token,
                    resetPasswordRequest.Password
                );

                if (isSuccess)
                {
                    logger.LogInformation("Senha redefinida com sucesso.");
                    return CreateResponse(true, nameof(ResetPassword), null);
                }
                else
                {
                    logger.LogWarning("Falha ao redefinir a senha: token inválido ou erro nos dados.");
                    return HandleBadRequest("Falha ao redefinir a senha. Verifique os dados e o token fornecido.");
                }
            }
            catch (ArgumentException ex)
            {
                // Captura o ArgumentException e retorna a mensagem de erro detalhada para o cliente
                logger.LogWarning(ex, "Erro ao redefinir a senha: dados inválidos.");
                return HandleBadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro ao redefinir a senha.");
                return HandleServerError("Um erro inesperado ocorreu ao redefinir a senha.");
            }
        }

    }
}
