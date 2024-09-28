using System.Text.Json.Serialization;

namespace ForumFuncionario.Api.Model.Response
{
    public class SsoResponse
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        [JsonIgnore]
        public UserApp AppUser { get; set; }

        public List<string> Roles { get; set; }

        [JsonConstructor]
        public SsoResponse()
        {
        }

        public SsoResponse(string accessToken, DateTime expiration, List<string> roles)
        {
            Token = accessToken;
            Expiration = expiration;
            Roles = roles;
        }

        public SsoResponse(string accessToken, DateTime expiration, List<string> roles, UserApp user)
        {
            Token = accessToken;
            Expiration = expiration;
            Roles = roles;
            AppUser = user;
        }

    }
}
