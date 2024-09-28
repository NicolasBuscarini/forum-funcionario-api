using ForumFuncionario.Api.Model.Entity;

namespace ForumFuncionario.Api.Repository.Interface
{
    public interface IUserProtheusRepository
    {
        Task<UserProtheus?> GetUserProtheusByUsernameAsync(string username);
    }
}