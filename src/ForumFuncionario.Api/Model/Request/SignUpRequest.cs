using System.ComponentModel.DataAnnotations;

namespace ForumFuncionario.Api.Model.Request
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        public required string ConfirmPassword { get; set; }

    }
}
