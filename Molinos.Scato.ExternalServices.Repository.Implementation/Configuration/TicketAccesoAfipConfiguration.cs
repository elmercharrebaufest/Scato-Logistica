using Microsoft.EntityFrameworkCore;
using Molinos.Scato.ExternalServices.Repository.Entities.Models;
using Molinos.Scato.ExternalServices.Repository.Implementation.Core;

namespace Molinos.Scato.ExternalServices.Repository.Implementation.Configuration
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