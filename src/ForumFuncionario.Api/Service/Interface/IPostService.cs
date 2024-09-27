using ForumFuncionario.Api.Model.Entity;

namespace ForumFuncionario.Api.Service.Interface
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(string title, string conteudo, string categoria, List<string> tags, string username);
        Task<List<Post>> GetLatestPostsByCategoriaAsync();
        Task<List<Post>> GetPostsByCategoriaAsync(string categoria);
    }
}