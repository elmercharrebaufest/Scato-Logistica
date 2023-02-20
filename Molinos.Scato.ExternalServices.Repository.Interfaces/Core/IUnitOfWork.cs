using Microsoft.EntityFrameworkCore;

namespace Molinos.Scato.ExternalServices.Repository.Interfaces.Core
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();

        void Dispose(bool disposing);

        T Repository<T>() where T : class;

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbContext Get();
    }
}