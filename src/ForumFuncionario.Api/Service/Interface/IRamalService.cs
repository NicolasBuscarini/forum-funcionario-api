using ForumFuncionario.Api.Model.Entity;

namespace ForumFuncionario.Api.Service.Interface
{
    public interface IRamalService
    {
        Task<List<Ramal>> GetRamal();
    }
}