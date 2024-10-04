namespace ForumFuncionario.Api.Model.Request
{
    public class ResetPasswordRequest
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }

    }
}
