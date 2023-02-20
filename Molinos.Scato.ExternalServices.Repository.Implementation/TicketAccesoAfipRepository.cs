using Microsoft.EntityFrameworkCore;
using Molinos.Scato.ExternalServices.Repository.Entities.Models;
using Molinos.Scato.ExternalServices.Repository.Implementation.Core;
using Molinos.Scato.ExternalServices.Repository.Interfaces;

namespace Molinos.Scato.ExternalServices.Repository.Implementation
{
    public class TicketAccesoAfipRepository : BaseRepository<TicketAccesoAfipEntity>, ITicketAccesoAfipRepository
    {
        private readonly DataContext context;
        public TicketAccesoAfipRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TicketAccesoAfipEntity>> ListarTicketActivo()
        {
            return await context.TicketAccesoAfip.Where(q => q.ExpirationTime > DateTime.Now).ToListAsync();
        }

        public async Task<TicketAccesoAfipEntity?> ObtenerTicket(int id)
        {
            return await context.TicketAccesoAfip.FirstOrDefaultAsync(q =>q.Id == id);
        }
    }
}