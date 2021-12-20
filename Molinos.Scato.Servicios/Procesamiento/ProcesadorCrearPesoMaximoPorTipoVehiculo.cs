using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearPesoMaximoPorTipoVehiculo : ProcesadorCrear<CrearPesoMaximoPorTipoVehiculo, PesoMaximoPorTipoVehiculo>
    {
        public ProcesadorCrearPesoMaximoPorTipoVehiculo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override PesoMaximoPorTipoVehiculo CrearEntidad(CrearPesoMaximoPorTipoVehiculo comando)
        {
            var entidad = Conversor.Convertir<PesoMaximoPorTipoVehiculoDto, PesoMaximoPorTipoVehiculo>(comando.Dto);
            entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return entidad;
        }

        protected override void Validar(CrearPesoMaximoPorTipoVehiculo comando, Resultado resultado)
        {
            if (Repositorio.Existe<PesoMaximoPorTipoVehiculo>(e => e.TipoVehiculo == comando.Dto.TipoVehiculo && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("", Textos.PesoMaximoPorTipoVehiculo_Existente);
            }
        }
    }
}
