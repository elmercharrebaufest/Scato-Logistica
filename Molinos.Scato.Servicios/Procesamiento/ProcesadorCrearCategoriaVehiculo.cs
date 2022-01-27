using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCategoriaVehiculo : ProcesadorCrear<CrearCategoriaVehiculo, CategoriaVehiculo>
    {
        public ProcesadorCrearCategoriaVehiculo(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override CategoriaVehiculo CrearEntidad(CrearCategoriaVehiculo comando)
        {
            return new CategoriaVehiculo
            {
                Patente = comando.Dto.Patente,
                PatenteAcoplado = comando.Dto.PatenteAcoplado,
                PatenteAcoplado2 = comando.Dto.PatenteAcoplado2,
                TipoVehiculo = comando.Dto.TipoVehiculo
            };
        }

        protected override void Validar(CrearCategoriaVehiculo comando, Resultado resultado)
        {
            if (
                Repositorio.Existe<CategoriaVehiculo>(
                    x =>
                    x.Id != comando.Dto.Id && x.Patente == comando.Dto.Patente))
            {
                resultado.Error("Patente", Textos.CategoriaCamiones_PatenteExistente);
            }
            if (
                Repositorio.Existe<CategoriaVehiculo>(
                    x =>
                    x.Id != comando.Dto.Id && x.PatenteAcoplado == comando.Dto.PatenteAcoplado))
            {
                resultado.Error("PatenteAcoplado", Textos.CategoriaCamiones_AcopladoExistente);
            }
            if (
                Repositorio.Existe<CategoriaVehiculo>(
                    x =>
                    x.Id != comando.Dto.Id && x.PatenteAcoplado2 == comando.Dto.PatenteAcoplado2))
            {
                resultado.Error("PatenteAcoplado2", Textos.CategoriaCamiones_Acoplado2Existente);
            }
        }
    }
}
