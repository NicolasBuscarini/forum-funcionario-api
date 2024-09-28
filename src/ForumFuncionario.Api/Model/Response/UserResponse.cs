namespace ForumFuncionario.Api.Model.Response
{
    public class UserResponse
    {
        public string UserName { get; set; }

        public UserResponse()
        {
        }

        public UserResponse(UserApp user)
        {
            UserName = user.UserName;
        }

    }
}
