using MicroService.Core.Entities;
using MongoDB.Driver;

namespace MicroService.Core.ContextMongoDb
{
    public interface IAutorContext
    {
        IMongoCollection<Autor> Autores { get; }
    }
}
