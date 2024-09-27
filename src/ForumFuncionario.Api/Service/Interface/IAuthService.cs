using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;

namespace ForumFuncionario.Api.Service.Interface
{
    public interface IAuthService
    {
        Task AddUserToAdminRole(int userId);
        Task<bool> DeleteUser(int userId);
        Task<AppUser> GetCurrentUser();
        Task<AppUser> GetUserById(int userId);
        Task<UserResponse> GetUserDto(int userId);
        Task<List<AppUser>> ListUsers();
        Task<SsoResponse> SignIn(SignInRequest signInDto);
        Task<bool> SignUp(SignUpRequest signUpDto);
        Task<int> UpdateUser(AppUser user);
    }
}