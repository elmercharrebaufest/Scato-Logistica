using Molinos.Scato.API.Infrastructure.Repository.Entities.Model;
using Molinos.Scato.API.Infrastructure.Repository.Interfaces.Core;

namespace Molinos.Scato.API.Infrastructure.Repository.Interfaces
{
    public interface ITicketAccesoAfipRepository : IBaseRepository<TicketAccesoAfipEntity>
    {
        Task<TicketAccesoAfipEntity?> ObtenerTicketActivo();
    }
}