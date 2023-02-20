using Molinos.Scato.ExternalServices.Repository.Entities.Models;
using Molinos.Scato.ExternalServices.Repository.Interfaces.Core;

namespace Molinos.Scato.ExternalServices.Repository.Interfaces
{
    public interface ITicketAccesoAfipRepository : IBaseRepository<TicketAccesoAfipEntity>
    {
        Task<TicketAccesoAfipEntity> ObtenerTicket(int id);

        Task<List<TicketAccesoAfipEntity>> ListarTicketActivo();
    }
}