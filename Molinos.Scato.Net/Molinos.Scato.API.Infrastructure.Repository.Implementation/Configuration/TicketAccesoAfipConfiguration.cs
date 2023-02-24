using Microsoft.EntityFrameworkCore;
using Molinos.Scato.API.Infrastructure.Repository.Entities.Model;
using Molinos.Scato.API.Infrastructure.Repository.Implementation.Core;

namespace Molinos.Scato.API.Infrastructure.Repository.Implementation.Configuration
{
    public class TicketAccesoAfipConfiguration : EntityConfiguration<TicketAccesoAfipEntity>
    {
        public TicketAccesoAfipConfiguration(ModelBuilder builder)
        {
            var entityBuilder = builder.Entity<TicketAccesoAfipEntity>();
            entityBuilder.ToTable("TicketAccesoAfip");
            entityBuilder.HasKey(c => c.Id);
            Configure(entityBuilder);
        }
    }
}