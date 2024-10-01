using ForumFuncionario.Api.Model.Enumerable;

namespace ForumFuncionario.Api.Model.Request
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Categoria { get; set; }
        public List<string> Tags { get; set; }

        public void Validate()
        {
            // Verificar se a categoria é válida
            if (!Enum.TryParse<CategoriaEnum>(Categoria, true, out _))
            {
                throw new ArgumentException($"Categoria '{Categoria}' não é válida.");
            }
        }
    }
}
