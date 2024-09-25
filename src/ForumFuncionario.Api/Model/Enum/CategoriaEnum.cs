using System.Text.Json.Serialization;

namespace ForumFuncionario.Api.Model.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CategoriaEnum
    {
        Rh,
        Suporte,
        Financeiro,
        Marketing,
        FiquePorDentro,
        Qualidade,
    }
}
