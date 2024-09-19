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

            // Chama o serviço para criar o post
            var post = await _postService.CreatePostAsync(request.Title, request.Body, request.Categoria, request.Tags);

            if (post == null)
            {
                return StatusCode(500, "Erro ao criar o post");
            }

            return Ok(post);
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
