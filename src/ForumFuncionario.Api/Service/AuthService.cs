using ForumFuncionario.Api.Repository.Interface;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ForumFuncionario.Api.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            // Obtém o usuário do banco de dados
            var user = await _authRepository.GetUserByUsernameAsync(username);

            if (user == null || !VerifyPassword(password, user.Password))
            {
                return null; // Retorna null se o login falhar
            }

            // Gera o token JWT se o login for bem-sucedido
            return GenerateJwtToken(username);
        }

        // Método para verificar a senha criptografada em MD5
        private static bool VerifyPassword(string providedPassword, string storedPassword)
        {
            string hashedPassword = ComputeMd5Hash(providedPassword);
            return hashedPassword == storedPassword;
        }

        // Método para criptografar em MD5
        private static string ComputeMd5Hash(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes);
        }

        // Método para gerar o token JWT
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
            return tokenHandler.WriteToken(token);
        }
    }
}
