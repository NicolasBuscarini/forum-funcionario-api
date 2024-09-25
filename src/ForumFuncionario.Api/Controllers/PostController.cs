using ForumFuncionario.Api.Model;
using ForumFuncionario.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        // POST api/post
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.Identity!.Name ?? "Anonymous";

            // Chama o serviço para criar o post
            var post = await _postService.CreatePostAsync(request.Title, request.Body, request.Categoria, request.Tags, username);

            if (post == null)
            {
                return StatusCode(500, "Erro ao criar o post");
            }

            return Ok(post);
        }

        // GET api/post/categoria/{categoria}
        [HttpGet("categoria/{categoria}")]
        [Authorize]
        public async Task<IActionResult> GetPostsByCategoria(string categoria)
        {
            var posts = await _postService.GetPostsByCategoriaAsync(categoria);

            if (posts == null || !posts.Any())
            {
                return NotFound("Nenhum post encontrado para essa categoria.");
            }

            return Ok(posts);
        }

        // GET api/post/recentes
        [HttpGet("recentes")]
        [Authorize]
        public async Task<IActionResult> GetLatestPostsByCategoria()
        {
            var latestPosts = await _postService.GetLatestPostsByCategoriaAsync();

            if (latestPosts == null || !latestPosts.Any())
            {
                return NotFound("Nenhum post recente encontrado.");
            }

            return Ok(latestPosts);
        }
    }

    // DTO para receber os dados da requisição
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Categoria { get; set; }
        public List<string> Tags { get; set; }
    }
}
