using ForumFuncionario.Api.Model.Enumerable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumFuncionario.Api.Model.Entity
{
    public class Ramal
    {
        [Key] // Define a chave prim�ria da tabela
        public int Id { get; set; }

        [Required] // O campo � obrigat�rio
        public string Nome { get; set; } 

        [Required] // O campo � obrigat�rio
        public string RamalNumber { get; set; } 

        
    }
}
