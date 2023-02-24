using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Molinos.Scato.API.CrossCutting.DTO;
using Molinos.Scato.API.CrossCutting.IoC;
using Molinos.Scato.API.Infrastructure.Repository.Interfaces.Core;

namespace Molinos.Scato.API.Infrastructure.Repository.Implementation.Core
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly AppSetting settings;
        private Dictionary<Type, object> repositories;
        private bool _disposed;
        public Func<DateTime> CurrentDateTime { get; set; } = () => DateTime.Now;

        public UnitOfWork(IOptions<AppSetting> settings)
        {
            this.settings = settings.Value;
            _context = new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlServer(this.settings.ConnectionStrings.DefaultConnection).Options);
            //_context = new DataContext(new DbContextOptionsBuilder<DataContext>().UseNpgsql(this.settings.ConnectionStrings.DefaultConnection).Options);
            //_httpContext = httpContext;
        }

        public DbContext Get()
        {
            return _context;
        }

        public T Repository<T>() where T : class
        {
            if (repositories == null)
                repositories = new Dictionary<Type, object>();

            var type = typeof(T);
            if (!repositories.ContainsKey(type))
            {
                repositories.Add(type, IoCContainer.Current.Resolve<T>("context", _context));
            }

            return (T)repositories[type];
        }

        public void SaveChanges()
        {
            TrackChanges();
            _context.SaveChanges();
        }

        private void TrackChanges()
        {
            if (_context.ChangeTracker.Entries().Any(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                foreach (var entry in _context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Metadata.FindProperty("CreatedDate") != null)
                            entry.CurrentValues["CreatedDate"] = CurrentDateTime();
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        if (entry.Metadata.FindProperty("ModifiedDate") != null)
                            entry.CurrentValues["ModifiedDate"] = CurrentDateTime();
                    }
                }
            }
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    if (repositories != null)
                        repositories.Clear();

                    _context.Dispose();
                }

            _disposed = true;
        }
    }
}