using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Request;

namespace ForumFuncionario.Api.Service.Interface
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(CreatePostRequest request, string username);
        Task<List<Post>> GetLatestPostsByCategoriaAsync();
        Task<List<Post>> GetPostsByCategoriaAsync(string categoria);
    }
}