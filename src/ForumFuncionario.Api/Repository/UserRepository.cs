using ForumFuncionario.Api.Context;
using ForumFuncionario.Api.Repository.Interface;

namespace ForumFuncionario.Api.Repository
{
    /// <summary>
    /// Repository implementation for user entities.
    /// </summary>
    public class UserRepository : GenericRepository<AppUser, int>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserRepository class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
            : base(context, logger)
        {
        }
    }
}
