using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    /// <summary>
    /// Controller for authentication-related operations.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </remarks>
    /// <param name="authCommand">The authentication command service.</param>
    /// <param name="logger">The logger.</param>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authCommand, ILogger<AuthController> logger) : ControllerBase
    {

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="signUpDto">The sign up data.</param>
        /// <returns>The result of the sign-up operation.</returns>
        [AllowAnonymous]
        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(ResponseCommon<bool>), 200)]
        public async Task<ActionResult> SignUp([FromBody] SignUpRequest signUpDto)
        {
            logger.LogInformation("Attempting to sign up user...");
            var response = await authCommand.SignUp(signUpDto);
            if (response)
            {
                logger.LogInformation("User signed up successfully.");
                return Ok(response);
            }
            else
            {
                logger.LogError($"Failed to sign up user.");
                return StatusCode(404, response);
            }
        }

        /// <summary>
        /// Signs in a user.
        /// </summary>
        /// <param name="signInDTO">The sign-in data.</param>
        /// <returns>The single sign-on data.</returns>
        [AllowAnonymous]
        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(ResponseCommon<SsoResponse>), 200)]
        public async Task<ActionResult> SignIn([FromBody] SignInRequest signInDTO)
        {
            logger.LogInformation("Attempting to sign in user...");
            var response = await authCommand.SignIn(signInDTO);
            if (response != null)
            {
                logger.LogInformation("User signed in successfully.");
                return Ok(response);
            }
            else
            {
                logger.LogError($"Failed to sign in user.");
                return StatusCode(404, response);
            }
        }

        /// <summary>
        /// Adds a user to the admin role.
        /// </summary>
        /// <param name="userId">The ID of the user to add.</param>
        /// <returns>True if the user was added successfully.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("add-user-to-admin-role")]
        [ProducesResponseType(typeof(ResponseCommon<bool>), 200)]
        public async Task<ActionResult> AddUserToAdminRole([FromBody] int userId)
        {
            logger.LogInformation($"Attempting to add user {userId} to admin role...");
            await authCommand.AddUserToAdminRole(userId);
            logger.LogInformation($"User {userId} added to admin role successfully.");
            return Ok();
        }

        /// <summary>
        /// Retrieves the current user.
        /// </summary>
        /// <returns>The current user.</returns>
        [HttpGet("get-current-user")]
        [ProducesResponseType(typeof(ResponseCommon<AppUser>), 200)]
        public async Task<ActionResult> GetCurrentUser()
        {
            logger.LogInformation("Attempting to get current user...");
            var response = await authCommand.GetCurrentUser();
            if (response != null)
            {
                logger.LogInformation("Current user retrieved successfully.");
                return Ok(response);
            }
            else
            {
                logger.LogError($"Failed to get current user.");
                return StatusCode(4044, response);
            }
        }

        /// <summary>
        /// Lists all users.
        /// </summary>
        /// <returns>The list of users.</returns>
        [HttpGet("list-users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseCommon<List<AppUser>>), 200)]
        public async Task<ActionResult> ListUsers()
        {
            logger.LogInformation("Attempting to list users...");
            var response = await authCommand.ListUsers();
            if (response != null)
            {
                logger.LogInformation("Users listed successfully.");
                return Ok(response);
            }
            else
            {
                logger.LogError($"Failed to list users.");
                return StatusCode(404, response);
            }
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user DTO.</returns>
        [HttpGet("get-userdto")]
        [ProducesResponseType(typeof(ResponseCommon<AppUser>), 200)]
        public async Task<ActionResult> GetUserDto([FromQuery] int id)
        {
            logger.LogInformation($"Attempting to get user with ID: {id}...");
            var response = await authCommand.GetUserDto(id);
            if (response != null)
            {
                logger.LogInformation($"User with ID: {id} retrieved successfully.");
                return Ok(response);
            }
            else
            {
                logger.LogError($"Failed to get user with ID {id}");
                return StatusCode(404, response);
            }
        }
    }
}
