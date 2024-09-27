using ForumFuncionario.Api.Model.Entity;

namespace ForumFuncionario.Api.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
    }
}