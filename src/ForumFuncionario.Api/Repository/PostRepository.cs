using ForumFuncionario.Api.Context;
using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;

namespace ForumFuncionario.Api.Repository
{
    public class PostRepository : GenericRepository<Post, int>, IPostRepository
    {
        public PostRepository(AppDbContext context, ILogger<PostRepository> logger)
            : base(context, logger)
        {
        }
    }
}
