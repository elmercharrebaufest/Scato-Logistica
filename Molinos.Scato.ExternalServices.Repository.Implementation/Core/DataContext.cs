using Microsoft.EntityFrameworkCore;
using Molinos.Scato.ExternalServices.Repository.Entities.Models;
using Molinos.Scato.ExternalServices.Repository.Implementation.Configuration;

namespace Molinos.Scato.ExternalServices.Repository.Implementation.Core
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