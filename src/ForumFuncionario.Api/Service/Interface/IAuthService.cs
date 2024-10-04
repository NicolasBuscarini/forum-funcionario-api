using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;

namespace ForumFuncionario.Api.Service.Interface
{
    public interface IAuthService
    {
        Task AddUserToAdminRole(int userId);
        Task<bool> DeleteUser(int userId);
        Task<UserApp> GetCurrentUser();
        Task<UserApp> GetUserById(int userId);
        Task<UserResponse> GetUserDto(int userId);
        Task<List<UserApp>> ListUsers();
        Task<SsoResponse> SignIn(SignInRequest signInDto, string clientIp);
        Task<bool> SignUp(SignUpRequest signUpDto);
        Task<int> UpdateUser(UserApp user);
        Task<UserApp?> GetUserAppByUsername(string username);
        Task<UserProtheus?> GetUserProtheusByUsername(string username);
        Task<bool> EsqueciSenha(string username);
        Task<bool> RedefinirSenhaAsync(string username, string resetToken, string newPassword);
    }
}