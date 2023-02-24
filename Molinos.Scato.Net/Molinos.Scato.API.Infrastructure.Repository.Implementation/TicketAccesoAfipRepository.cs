using Microsoft.EntityFrameworkCore;
using Molinos.Scato.API.Infrastructure.Repository.Entities.Model;
using Molinos.Scato.API.Infrastructure.Repository.Implementation.Core;
using Molinos.Scato.API.Infrastructure.Repository.Interfaces;

namespace  Molinos.Scato.API.Infrastructure.Repository.Implementation
{
    public class TicketAccesoAfipRepository : BaseRepository<TicketAccesoAfipEntity>, ITicketAccesoAfipRepository
    {
        private readonly DataContext context;

        public TicketAccesoAfipRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<TicketAccesoAfipEntity?> ObtenerTicketActivo()
        {
            var result = await context.TicketAccesoAfip.FirstOrDefaultAsync(q => q.ExpirationTime > DateTime.Now);
            return result;
        }
    }
}