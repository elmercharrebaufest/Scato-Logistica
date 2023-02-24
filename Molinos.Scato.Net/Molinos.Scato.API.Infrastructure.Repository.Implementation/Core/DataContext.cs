using Microsoft.EntityFrameworkCore;
using Molinos.Scato.API.Infrastructure.Repository.Entities.Model;
using Molinos.Scato.API.Infrastructure.Repository.Implementation.Configuration;

namespace Molinos.Scato.API.Infrastructure.Repository.Implementation.Core
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TicketAccesoAfipConfiguration(builder));
        }

        public DbSet<TicketAccesoAfipEntity> TicketAccesoAfip { get; set; }
    }
}