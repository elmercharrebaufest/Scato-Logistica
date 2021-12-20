using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarPesoVehiculo : ProcesadorModificar<ModificarPesoVehiculo>
    {
        public ProcesadorModificarPesoVehiculo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarPesoVehiculo comando)
        {
            var entidad = Repositorio.Obtener<Recorrido>( x => x.InstanciaWorkflow == comando.Id).Vehiculo;
            entidad.PesoBrutoOrigen = comando.PesoBruto;
            entidad.PesoTaraOrigen = comando.PesoTara;
            entidad.PesoNetoOrigen = comando.PesoBruto - comando.PesoTara;
        }

        protected override void Validar(ModificarPesoVehiculo comando, Resultado resultado)
        {
        }
    }
}
