using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace ForumFuncionario.Api.Model.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategoriaEnum
    {
        Rh,
        Infra,
        Admissao,
        Financeiro,
        Marketing,
        Tecnologia,
        Comercial,
        Juridico,
        Operacional,
        Treinamento,
        Compras,
        Vendas,
        Atendimento,
        Projetos,
        Qualidade,
        PesquisaDesenvolvimento
    }
}
