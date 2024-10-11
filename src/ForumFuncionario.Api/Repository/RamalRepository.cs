using ForumFuncionario.Api.Context;
using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Repository.Interface;

namespace ForumFuncionario.Api.Repository
{
    public class RamalRepository : GenericRepository<Ramal, int>, IRamalRepository
    {
        public RamalRepository(AppDbContext context, ILogger<RamalRepository> logger)
            : base(context, logger)
        {
        }
    }
}
