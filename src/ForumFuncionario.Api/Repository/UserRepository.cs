using ForumFuncionario.Api.Context;
using ForumFuncionario.Api.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ForumFuncionario.Api.Repository
{
    /// <summary>
    /// Repository implementation for user entities.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the UserRepository class.
    /// </remarks>
    /// <param name="context">The application database context.</param>
    public class UserRepository(AppDbContext context, ILogger<UserRepository> logger) : GenericRepository<UserApp, int>(context, logger), IUserRepository
    {

        /// <summary>
        /// Retrieves a user by their username asynchronously.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>The user with the specified username, or null if not found.</returns>
        public async Task<UserApp?> GetUserByUsernameAsync(string username)
        {
            try
            {
                // Query the UserApp table for the user with the specified username
                return await context.Users
                    .FirstOrDefaultAsync(u => u.UserName == username);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred while retrieving user by username: {username}.");
                throw;
            }
        }
    }
}
