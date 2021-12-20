using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearVehiculo : ProcesadorCrear<CrearVehiculo, Vehiculo>
    {
        public ProcesadorCrearVehiculo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Vehiculo CrearEntidad(CrearVehiculo comando)
        {
            var vehiculo = Conversor.Convertir<VehiculoDto, Vehiculo>(comando.Dto);
            return vehiculo;
        }

        protected override void Validar(CrearVehiculo comando, Resultado resultado)
        {

        }
    }
}
