using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Model.Response;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumFuncionario.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : BaseController(logger)
    {
        /// <summary>
        /// Registers a new user.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("sign-up")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]   // Success
        [ProducesResponseType(typeof(BaseResponse<string>), 500)] // Internal Server Error
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest signUpDto)
        {
            try
            {
                _logger.LogInformation("Attempting to sign up user...");
                var response = await authService.SignUp(signUpDto);
                if (response)
                {
                    _logger.LogInformation("User signed up successfully.");
                    return CreateResponse(response, nameof(SignUp), null);
                }
                else
                {
                    return HandleServerError("Failed to sign up user.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while signing up the user.");
                return HandleServerError("An unexpected error occurred during user sign-up.");
            }
        }

        /// <summary>
        /// Signs in a user.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("sign-in")]
        [ProducesResponseType(typeof(BaseResponse<SsoResponse>), 200)]   // Success
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]        // Not Found
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]        // Internal Server Error
        public async Task<IActionResult> SignIn([FromBody] SignInRequest signInDTO)
        {
            try
            {
                _logger.LogInformation("Attempting to sign in user...");
                var response = await authService.SignIn(signInDTO);
                if (response != null)
                {
                    _logger.LogInformation("User signed in successfully.");
                    return CreateResponse(response, nameof(SignIn), null);
                }
                else
                {
                    return HandleNotFound<SsoResponse>("Failed to sign in user.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while signing in the user.");
                return HandleServerError("An unexpected error occurred during user sign-in.");
            }
        }

        /// <summary>
        /// Adds a user to the admin role.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("add-user-to-admin-role")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]   // Success
        [ProducesResponseType(typeof(BaseResponse<string>), 500)] // Internal Server Error
        public async Task<IActionResult> AddUserToAdminRole([FromBody] int userId)
        {
            try
            {
                _logger.LogInformation($"Attempting to add user {userId} to admin role...");
                await authService.AddUserToAdminRole(userId);
                _logger.LogInformation($"User {userId} added to admin role successfully.");
                return CreateResponse<bool>(true, nameof(AddUserToAdminRole), new { userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding user {userId} to the admin role.");
                return HandleServerError($"An unexpected error occurred while adding user {userId} to admin role.");
            }
        }

        /// <summary>
        /// Retrieves the current user.
        /// </summary>
        [HttpGet("get-current-user")]
        [ProducesResponseType(typeof(BaseResponse<AppUser>), 200)]   // Success
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]    // Not Found
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]    // Internal Server Error
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                _logger.LogInformation("Attempting to get current user...");
                var response = await authService.GetCurrentUser();
                if (response != null)
                {
                    _logger.LogInformation("Current user retrieved successfully.");
                    return CreateResponse(response, nameof(GetCurrentUser), null);
                }
                else
                {
                    return HandleNotFound<AppUser>("Failed to get current user.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the current user.");
                return HandleServerError("An unexpected error occurred while getting current user.");
            }
        }

        /// <summary>
        /// Lists all users.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("list-users")]
        [ProducesResponseType(typeof(BaseResponse<List<AppUser>>), 200)]   // Success
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]          // Not Found
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]          // Internal Server Error
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                _logger.LogInformation("Attempting to list users...");
                var response = await authService.ListUsers();
                if (response != null)
                {
                    _logger.LogInformation("Users listed successfully.");
                    return CreateResponse(response, new MetaData(response.Count, response.Count, 1, 1), nameof(ListUsers), null);
                }
                else
                {
                    return HandleNotFound<AppUser>("Failed to list users.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while listing users.");
                return HandleServerError("An unexpected error occurred while listing users.");
            }
        }

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        [HttpGet("get-userdto")]
        [ProducesResponseType(typeof(BaseResponse<UserResponse>), 200)]   // Success
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]         // Not Found
        [ProducesResponseType(typeof(BaseResponse<string>), 500)]         // Internal Server Error
        public async Task<IActionResult> GetUserDto([FromQuery] int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to get user with ID: {id}...");
                var response = await authService.GetUserDto(id);
                if (response != null)
                {
                    _logger.LogInformation($"User with ID: {id} retrieved successfully.");
                    return CreateResponse(response, nameof(GetUserDto), new { id });
                }
                else
                {
                    return HandleNotFound<AppUser>($"Failed to get user with ID {id}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving user with ID {id}.");
                return HandleServerError($"An unexpected error occurred while getting user with ID {id}.");
            }
        }
    }
}
