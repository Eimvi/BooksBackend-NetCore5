using MicroService.Core.ContextMongoDb;
using MicroService.Core.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.Repository
{
    public class AutorRepository : IAutorRepository
    {
        private readonly IAutorContext _autorContext;
        public AutorRepository(IAutorContext autorContext)
        {
            _autorContext = autorContext;
        }
        public async Task<IEnumerable<Autor>> GetAutores()
        {
            return await _autorContext.Autores.Find(prop => true).ToListAsync();
        }
    }
}
