using MicroService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroService.Repository
{
    public interface IAutorRepository
    {
        Task<IEnumerable<Autor>> GetAutores();
    }
}