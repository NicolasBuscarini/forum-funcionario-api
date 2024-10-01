using ForumFuncionario.Api.Model.Enumerable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumFuncionario.Api.Model.Entity
{
    public class Post
    {
        [Key] // Define a chave primária da tabela
        public int Id { get; set; }

        [Required] // O campo é obrigatório
        public string Titulo { get; set; } // Título da postagem

        [Required] // O campo é obrigatório
        public string Conteudo { get; set; } // Conteúdo da postagem

        [Required]
        public string Autor { get; set; } // Nome ou identificador do autor da postagem

        [Required]
        public DateTime DataCriacao { get; set; } // Data de criação da postagem

        public DateTime? DataEdicao { get; set; } // Data de edição da postagem (opcional)

        public bool Ativo { get; set; } = true; // Indica se a postagem está ativa ou não (para exclusão lógica)

        [Required]
        [Column(TypeName = "varchar(50)")]
        public CategoriaEnum Categoria { get; set; } // Categoria da postagem

        public List<string> Tags { get; set; } = []; // Lista de tags relacionadas à postagem

    }
}
