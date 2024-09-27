using System.ComponentModel.DataAnnotations;

namespace ForumFuncionario.Api.Model.Request
{
    public class SignInRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public SignInRequest()
        {
            Username = "";
            Password = "";
        }

        public SignInRequest(string username, string password)
        {
            Username = username;
            Password = password;
        }

    }
}
