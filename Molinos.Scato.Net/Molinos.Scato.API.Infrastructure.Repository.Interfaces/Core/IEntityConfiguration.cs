using Microsoft.EntityFrameworkCore;

namespace Molinos.Scato.API.Infrastructure.Repository.Interfaces.Core
{
    public interface IEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class
    {
    }
}