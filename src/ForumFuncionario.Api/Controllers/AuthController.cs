using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ForumFuncionario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("signin")]
        public IActionResult SignIn([FromBody] LoginRequest request)
        {
            // Verifica se os parâmetros foram fornecidos
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                _logger.LogWarning("Tentativa de login falhou: Nome de usuário ou senha ausentes.");
                return BadRequest(new { Mensagem = "Nome de usuário ou senha não podem estar vazios" });
            }

            // Autentica no AD usando LDAP
            _logger.LogInformation("Iniciando autenticação para o usuário: {Username}", request.Username);
            using PrincipalContext context = new PrincipalContext(ContextType.Machine);
            bool isValid = context.ValidateCredentials(request.Username, request.Password);

            if (isValid)
            {
                // Recupera informações adicionais do usuário
                using UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, request.Username);

                if (userPrincipal != null)
                {
                    var userInfo = new
                    {
                        Username = userPrincipal.SamAccountName,    // Nome de usuário
                        FullName = userPrincipal.DisplayName,       // Nome completo
                        Email = userPrincipal.EmailAddress,         // Email do usuário
                        Groups = userPrincipal.GetAuthorizationGroups().Select(g => g.Name).ToList() // Grupos do AD
                    };

                    var token = GenerateJwtToken(userPrincipal.SamAccountName);
                    _logger.LogInformation("Usuário {Username} autenticado com sucesso.", request.Username);

                    return Ok(new
                    {
                        Mensagem = "Usuário autenticado com sucesso",
                        Token = token,
                        DetalhesDoUsuario = userInfo // Retorna as informações adicionais do usuário
                    });
                }
                else
                {
                    _logger.LogError("Falha ao recuperar as informações do usuário {Username}.", request.Username);
                }
            }
            else
            {
                _logger.LogWarning("Tentativa de login inválida para o usuário {Username}.", request.Username);
            }
            return Unauthorized(new { Mensagem = "Nome de usuário ou senha inválidos" });
        }

        [HttpGet("test-jwt")]
        [Authorize]
        public IActionResult TestJwt()
        {
            // Verifica se o token JWT foi passado e se o usuário está autenticado
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name; // Obtém o nome do usuário do token
                var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(); // Obtém todas as claims do token

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

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyString = _configuration["Jwt:Key"];
            var key = Encoding.UTF8.GetBytes(keyString!);

            var authSigningKey = new SymmetricSecurityKey(key);
            var expiresTime = DateTime.UtcNow.AddHours(3);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: expiresTime,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            _logger.LogInformation("Token JWT gerado com sucesso para o usuário: {Username}", username);
            return tokenHandler.WriteToken(token);
        }
    }

    public record LoginRequest(string Username, string Password);
}
