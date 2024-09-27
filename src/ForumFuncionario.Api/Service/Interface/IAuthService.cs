namespace ForumFuncionario.Api.Service.Interface
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(string username, string password);
    }
}