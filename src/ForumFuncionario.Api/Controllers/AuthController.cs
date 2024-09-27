using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ForumFuncionario.Api.Service;

namespace ForumFuncionario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest request)
        {
            // Verifica se os parâmetros foram fornecidos
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Tentativa de login falhou: Nome de usuário ou senha ausentes.");
                return BadRequest(new { Mensagem = "Nome de usuário ou senha não podem estar vazios" });
            }

            var token = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (token == null)
            {
                _logger.LogWarning("Tentativa de login inválida para o usuário {Username}.", request.Username);
                return Unauthorized(new { Mensagem = "Nome de usuário ou senha inválidos" });
            }

            _logger.LogInformation("Usuário {Username} autenticado com sucesso.", request.Username);
            return Ok(new
            {
                Mensagem = "Usuário autenticado com sucesso",
                Token = token
            });
        }

        [HttpGet("test-jwt")]
        [Authorize]
        public IActionResult TestJwt()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

                _logger.LogInformation("Token JWT validado com sucesso para o usuário: {UserName}", userName);

                return Ok(new
                {
                    Mensagem = "JWT é válido",
                    NomeUsuario = userName,
                    Claims = userClaims
                });
            }

            _logger.LogWarning("Token JWT inválido ou ausente na tentativa de acesso.");
            return Unauthorized(new { Mensagem = "Token JWT inválido ou ausente" });
        }
    }

    public record LoginRequest(string Username, string Password);
}
