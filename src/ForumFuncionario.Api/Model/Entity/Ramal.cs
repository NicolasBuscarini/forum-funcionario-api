using ForumFuncionario.Api.Model.Enumerable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumFuncionario.Api.Model.Entity
{
    public class Ramal
    {
        [Key] // Define a chave primária da tabela
        public int Id { get; set; }

        [Required] // O campo é obrigatório
        public string Nome { get; set; } 

        [Required] // O campo é obrigatório
        public string RamalNumber { get; set; } 

        
    }
}
