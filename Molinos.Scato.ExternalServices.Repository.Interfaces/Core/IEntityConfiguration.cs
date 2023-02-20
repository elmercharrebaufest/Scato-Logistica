using Microsoft.EntityFrameworkCore;

namespace Molinos.Scato.ExternalServices.Repository.Interfaces.Core
{
    public interface IEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class
    {
    }
}