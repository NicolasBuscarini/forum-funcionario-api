namespace ForumFuncionario.Api.Repository.Interface
{
    /// <summary>
    /// Interface for the repository of users.
    /// </summary>
    public interface IUserRepository : IGenericRepository<UserApp, int>
    {
        Task<UserApp?> GetUserByUsernameAsync(string username);
    }
}
