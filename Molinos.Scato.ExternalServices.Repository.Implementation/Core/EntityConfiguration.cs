using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Molinos.Scato.ExternalServices.Repository.Interfaces.Core;

namespace Molinos.Scato.ExternalServices.Repository.Implementation.Core
{
    public abstract class EntityConfiguration<T> : IEntityConfiguration<T> where T : class
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property("Id").HasColumnName("Id").UseIdentityColumn();
            //builder.Property("CreatedDate").HasColumnName("CreatedDate");
            //builder.Property("ModifiedDate").HasColumnName("ModifiedDate");
            //builder.Property("IsDeleted").HasColumnName("IsDeleted");
        }
    }
}