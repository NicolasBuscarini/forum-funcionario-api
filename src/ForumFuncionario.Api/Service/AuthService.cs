using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;
using ForumFuncionario.Api.Repository.Interface;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace ForumFuncionario.Api.Service
{
    /// <summary>
    /// Service implementation for authentication-related operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </remarks>
    /// <param name="logger">Logger instance.</param>
    /// <param name="userRepository">User repository instance.</param>
    /// <param name="configuration">Configuration instance.</param>
    /// <param name="userManager">User manager wrapper instance.</param>
    /// <param name="httpContextAccessor">HTTP context accessor instance.</param>
    public class AuthService(
        ILogger<AuthService> logger,
        IUserRepository userRepository,
        IConfiguration configuration,
        UserManager<UserApp> userManager,
        IHttpContextAccessor httpContextAccessor,
        IUserProtheusRepository userProtheusRepository,
        IEmailService emailService) : IAuthService
    {

        /// <summary>
        /// Retrieves a user from Protheus by their username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The user from Protheus if found, otherwise null.</returns>
        public async Task<UserProtheus?> GetUserProtheusByUsername(string username)
        {
            try
            {
                return await userProtheusRepository.GetUserProtheusByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving Protheus user by username: {username}.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a user from the application by their username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The user from the application if found, otherwise null.</returns>
        public async Task<UserApp?> GetUserAppByUsername(string username)
        {
            try
            {
                return await userRepository.GetUserByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving application user by username: {username}.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        public async Task<List<UserApp>> ListUsers()
        {
            try
            {
                List<UserApp> listUsers = await userRepository.ListAll().ToListAsync();
                return listUsers;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while listing users.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>The user with the specified ID.</returns>
        public async Task<UserApp> GetUserById(int userId)
        {
            try
            {
                UserApp user = await userRepository.GetByIdAsync(userId);
                return user ?? throw new ArgumentException("User does not exist.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving user by ID: {userId}.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a user DTO by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve the DTO for.</param>
        /// <returns>The DTO representing the user with the specified ID.</returns>
        public async Task<UserResponse> GetUserDto(int userId)
        {
            try
            {
                var user = await GetUserById(userId);
                UserResponse userDto = new(user);
                return userDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving user DTO by ID: {userId}.");
                throw;
            }
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The number of affected rows.</returns>
        public async Task<int> UpdateUser(UserApp user)
        {
            try
            {
                UserApp findUser = await userRepository.GetByIdAsync(user.Id) ?? throw new ArgumentException("User not found.");
                findUser.Email = user.Email;
                findUser.UserName = user.UserName;
                return await userRepository.UpdateAsync(findUser, findUser.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while updating user: {user.Id}.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>True if the user was deleted successfully; otherwise, false.</returns>
        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                UserApp findUser = await userRepository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found.");
                await userRepository.DeleteAsync(findUser, findUser.Id);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while deleting user: {userId}.");
                throw;
            }
        }

        /// <summary>
        /// Signs up a new user.
        /// </summary>
        /// <param name="signUpDto">The DTO containing sign up information.</param>
        /// <returns>True if sign up was successful; otherwise, false.</returns>
        public async Task<bool> SignUp(SignUpRequest signUpDto)
        {
            try
            {
                // Verifica se o funcionário existe na tabela de funcionários
                var userProtheus = await userProtheusRepository.GetUserProtheusByUsernameAsync(signUpDto.Username);
                if (userProtheus == null)
                {
                    throw new ArgumentException("Usuário não possui cadastro na tabela de funcionários.");
                }

                // Verifica se o nome de usuário já existe
                var userExists = await userManager.FindByNameAsync(signUpDto.Username);
                if (userExists != null)
                {
                    throw new ArgumentException("Nome de usuário já existe.");
                }

                // Cria um novo usuário
                UserApp user = new()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = signUpDto.Username,
                    UserProtheusId = userProtheus.ProtheusId,
                    RaNome = signUpDto.Username,
                    Email = userProtheus.Email
                };

                var result = await userManager.CreateAsync(user, signUpDto.Password);

                if (!result.Succeeded)
                {
                    if (result.Errors.ToList().Count > 0)
                    {
                        throw new ArgumentException(result.Errors.ToList()[0].Description);
                    }
                    else
                    {
                        throw new ArgumentException("Falha no cadastro do usuário.");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro durante o cadastro de usuário.");
                throw;
            }
        }

        /// <summary>
        /// Adds a user to the administrator role.
        /// </summary>
        /// <param name="userId">The ID of the user to add to the administrator role.</param>
        public async Task AddUserToAdminRole(int userId)
        {
            try
            {
                var boolProperties = typeof(UserApp).GetProperties().Where(p => p.PropertyType == typeof(bool));
                await AddUserToRoleAsync(userId, "Admin", boolProperties);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while adding user {userId} to admin role.");
                throw;
            }
        }

        private async Task AddUserToRoleAsync(int userId, string roleName, IEnumerable<PropertyInfo>? propertyInfos = null)
        {
            try
            {
                UserApp user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new ArgumentException("User not found.");
                await userManager.AddToRoleAsync(user, roleName);

                //if (propertyInfos != null)
                //{
                //    // Iterate over the boolean properties and set them to true for the user
                //    foreach (var property in propertyInfos)
                //    {
                //        property.SetValue(property, true);
                //    }
                //}

                // Update the user in the database
                await userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while adding user {userId} to role {roleName}.");
                throw;
            }
        }

        /// <summary>
        /// Signs in a user.
        /// </summary>
        /// <param name="signInDto">The DTO containing sign in information.</param>
        /// <param name="clientIp"></param>
        /// <returns>The SSO DTO containing the authentication token and user information.</returns>        
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<SsoResponse> SignIn(SignInRequest signInDto, string clientIp)
        {
            try
            {
                var user = await userManager.FindByNameAsync(signInDto.Username) ?? throw new ArgumentException("User not found.");

                if (!await userManager.CheckPasswordAsync(user, signInDto.Password))
                    throw new ArgumentException("Invalid password.");

                var userRolesList = (await userManager.GetRolesAsync(user)).ToList();

                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName!),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email!),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new("client_ip", clientIp) // Custom claim for client IP
                };

                authClaims.AddRange(userRolesList.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
                var expiresTime = DateTime.UtcNow.AddHours(3);

                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    expires: expiresTime,
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new SsoResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresTime, userRolesList, user, clientIp);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during user sign in.");
                throw;
            }
        }

        /// <summary>
        /// Sends a password reset email with a link to the user.
        /// </summary>
        /// <param name="username">The email address of the user requesting the password reset.</param>
        /// <returns>True if the email was successfully sent; otherwise, false.</returns>
        public async Task<bool> EsqueciSenha(string username)
        {
            try
            {
                // Verifica se o email pertence a algum usuário
                var user = await userManager.FindByNameAsync(username);
                if (user == null)
                {
                    throw new ArgumentException("Usuário não encontrado.");
                }

                // Gera um token de redefinição de senha
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

                // Codifica o token para ser seguro em URLs
                var encodedToken = HttpUtility.UrlEncode(resetToken);

                // Cria o link de redefinição de senha
                var resetLink = $"{configuration["Frontend"]}/reset-password?token={encodedToken}&username={user.UserName}";

                // Envia o email com o link de redefinição de senha
                var emailSent = await emailService.SendEmailAsync(user.Email, "Recuperação de Senha",
                    $"Clique no link abaixo para redefinir sua senha: <a href='{resetLink}'>Redefinir Senha</a>");

                if (!emailSent)
                {
                    throw new Exception("Falha ao enviar o email de recuperação.");
                }

                logger.LogInformation($"Link de recuperação de senha enviado para o email: {user.Email}.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao processar recuperação de senha para o username: {username}.");
                throw;
            }
        }

        /// <summary>
        /// Resets the user's password after verifying the reset token.
        /// </summary>
        /// <param name="username">The email address of the user requesting password reset.</param>
        /// <param name="resetToken">The password reset token provided by the user.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>True if the password was successfully reset; otherwise, false.</returns>
        public async Task<bool> RedefinirSenhaAsync(string username, string resetToken, string newPassword)
        {
            try
            {
                // Decodifica o token recebido da URL
                //var decodedToken = HttpUtility.UrlDecode(resetToken);

                // Verifica se o email pertence a algum usuário
                var user = await userManager.FindByNameAsync(username);
                if (user == null)
                {
                    throw new ArgumentException("Usuário não encontrado.");
                }

                // Verifica e redefine a senha do usuário
                var result = await userManager.ResetPasswordAsync(user, resetToken, newPassword);

                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogWarning($"Falha ao redefinir a senha para o usuário: {username}. Motivo: {errorMessage}");
                    throw new ArgumentException($"Falha ao redefinir a senha. Motivo: {errorMessage}");
                }

                logger.LogInformation($"Senha redefinida com sucesso para o usuário: {username}.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao redefinir a senha para o username: {username}.");
                throw;
            }
        }


        /// <summary>
        /// Retrieves the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user.</returns>
        public async Task<UserApp> GetCurrentUser()
        {
            try
            {
                UserApp user = (await userManager.GetUserAsync(httpContextAccessor.HttpContext!.User))!;
                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving the current user.");
                throw;
            }
        }
    }
}
