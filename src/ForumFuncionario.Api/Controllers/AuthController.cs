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

            // Obtém o NOME_DO_DOMINIO da variável de ambiente ou do appsettings.json
            string domainName = _configuration["AD:DomainName"] ?? "WORKGROUP";

            if (string.IsNullOrEmpty(domainName))
            {
                _logger.LogError("NOME_DO_DOMINIO não configurado.");
                return StatusCode(500, new { Mensagem = "Erro interno: NOME_DO_DOMINIO não configurado." });
            }

            // Autentica no AD usando LDAP
            _logger.LogInformation("Iniciando autenticação para o usuário: {Username}", request.Username);
            using PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName);
            bool isValid = context.ValidateCredentials(request.Username, request.Password);

            if (isValid)
            {
                try
                {
                    // Simulando a recuperação de informações adicionais do usuário (opcional)
                    var userInfo = new
                    {
                        Username = request.Username
                        // Adicione outras informações conforme necessário
                    };

                    var token = GenerateJwtToken(request.Username);
                    _logger.LogInformation("Usuário {Username} autenticado com sucesso.", request.Username);

                    return Ok(new
                    {
                        Mensagem = "Usuário autenticado com sucesso",
                        Token = token,
                        DetalhesDoUsuario = userInfo
                    });
                }
                catch (Exception ex) // Tratamento de exceção
                {
                    _logger.LogError(ex, "Falha ao recuperar as informações do usuário {Username}.", request.Username);
                    return StatusCode(500, new { Mensagem = "Erro interno ao processar a solicitação" });
                }
            }
            else
            {
                _logger.LogWarning("Tentativa de login inválida para o usuário {Username}.", request.Username);
                return Unauthorized(new { Mensagem = "Nome de usuário ou senha inválidos" });
            }
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

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyString = _configuration["Jwt:Key"];
            var key = Encoding.UTF8.GetBytes(keyString!);

            var authSigningKey = new SymmetricSecurityKey(key);
            var expiresTime = DateTime.UtcNow.AddHours(3);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = expiresTime,
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            _logger.LogInformation("Token JWT gerado com sucesso para o usuário: {Username}", username);

            return tokenHandler.WriteToken(token);
        }
    }

    public record LoginRequest(string Username, string Password);
}
