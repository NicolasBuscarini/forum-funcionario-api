namespace ForumFuncionario.Api.Model.Request
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Categoria { get; set; }
        public List<string> Tags { get; set; }
    }
}
