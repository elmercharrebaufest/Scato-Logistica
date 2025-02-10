using FluentValidation;
using FluentValidation.Results;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Validations.Interfaces
{
    public interface IValidatorEntity<T> : IValidator<T> {}
}
