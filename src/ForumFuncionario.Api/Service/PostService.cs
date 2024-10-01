using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Enumerable;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Repository.Interface;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace ForumFuncionario.Api.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(IPostRepository postRepository, IHttpContextAccessor httpContextAccessor)
        {
            _postRepository = postRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Post> CreatePostAsync(CreatePostRequest request, string username)
        {
            request.Validate();

            var post = new Post
            {
                Ativo = true,
                Titulo = request.Title,
                Conteudo = request.Body,
                Categoria = Enum.Parse<CategoriaEnum>(request.Categoria),
                Tags = request.Tags,
                Autor = username,
                DataCriacao = DateTime.UtcNow
            };

            return await _postRepository.CreateAsync(post);
        }

        public async Task<List<Post>> GetPostsByCategoriaAsync(string categoria)
        {
            // Tente analisar a string de categoria para CategoriaEnum
            if (!Enum.TryParse<CategoriaEnum>(categoria, true, out var categoriaEnum))
            {
                throw new ArgumentException($"Categoria '{categoria}' não é válida.");
            }

            // Usando o repositório para buscar os posts pela categoria
            return await _postRepository.ListAll()
                    .Where(post => post.Categoria == categoriaEnum)
                    .OrderByDescending(c => c.DataCriacao)
                    .ToListAsync();
        }

        public async Task<List<Post>> GetLatestPostsByCategoriaAsync()
        {
            // Agrupa os posts por categoria e seleciona o mais recente de cada grupo
            var latestPosts = await _postRepository.ListAll()
                .GroupBy(post => post.Categoria)
                .Select(group => group.OrderByDescending(post => post.DataCriacao).FirstOrDefault())
                .ToListAsync();

            return latestPosts!;
        }
    }
}
