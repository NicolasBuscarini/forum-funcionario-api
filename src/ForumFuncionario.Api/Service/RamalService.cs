using ForumFuncionario.Api.Model.Entity;
using ForumFuncionario.Api.Model.Enumerable;
using ForumFuncionario.Api.Model.Request;
using ForumFuncionario.Api.Repository.Interface;
using ForumFuncionario.Api.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace ForumFuncionario.Api.Service
{
    public class RamalService : IRamalService
    {
        private readonly IRamalRepository _RamalRepository;

        public RamalService(IRamalRepository RamalRepository)
        {
            _RamalRepository = RamalRepository;
        }

        public async Task<List<Ramal>> GetRamal()
        {
            return await _RamalRepository.ListAll()
                    .ToListAsync();
        }
    }
}
