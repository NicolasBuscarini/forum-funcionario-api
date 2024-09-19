using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Enum;
using ForumFuncionario.Api.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ForumFuncionario.Api.Service
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(string title, string body, string categoria, List<string> tags);
    }

    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor)
        {
            _postRepository = postRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Post> CreatePostAsync(string title, string conteudo, string categoria, List<string> tags)
        {
            // Extract the name of the user from JWT token
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";

            // Try to parse the categoria string to CategoriaEnum
            if (!Enum.TryParse<CategoriaEnum>(categoria, true, out var categoriaEnum))
            {
                throw new ArgumentException($"Categoria '{categoria}' is not valid.");
            }

            var post = new Post
            {
                Ativo = true,
                Titulo = title,
                Conteudo = conteudo,
                Categoria = categoriaEnum,
                Tags = tags,
                Autor = userName,
                DataCriacao = DateTime.UtcNow
            };

            return await _postRepository.CreateAsync(post);
        }

    }
}
