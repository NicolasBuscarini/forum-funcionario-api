namespace ForumFuncionario.Api.Model.Response
{
    public class VerificarCadastroResponse
    {
        public bool Login {  get; set; }
        public required string Status { get; set; }
        public required string Mensagem { get; set; }
    }
}
