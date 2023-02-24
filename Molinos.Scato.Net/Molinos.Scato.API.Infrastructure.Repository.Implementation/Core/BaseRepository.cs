using Microsoft.EntityFrameworkCore;
using Molinos.Scato.API.Infrastructure.Repository.Interfaces.Core;
using System.Linq.Expressions;

namespace Molinos.Scato.API.Infrastructure.Repository.Implementation.Core
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private DbSet<T> table = null;
        private readonly DataContext context;

        public BaseRepository(DataContext context)
        {
            this.context = context;
            table = context.Set<T>();
        }

        public async Task<T> GetById(int id) => await table.FindAsync(id);

        public Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate) => table.FirstOrDefaultAsync(predicate);

        public T Add(T entity)
        {
            table.Add(entity);
            return entity;
        }

        public void Update(T entity)
        {
            table.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            table.Attach(entity);
            context.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await table.Where(predicate).ToListAsync();
        }

        public Task<int> CountAll() => table.CountAsync();

        public Task<int> CountWhere(Expression<Func<T, bool>> predicate) => table.CountAsync(predicate);
    }
}