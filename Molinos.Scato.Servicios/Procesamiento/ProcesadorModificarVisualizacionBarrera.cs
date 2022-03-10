using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarVisualizacionBarrera : ProcesadorModificar<ModificarVisualizacionBarrera>
    {
        public ProcesadorModificarVisualizacionBarrera(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarVisualizacionBarrera comando)
        {
            var visualizacionBarrera = Repositorio.Obtener<VisualizacionBarrera>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, visualizacionBarrera);

            if(visualizacionBarrera.Rol.Id != comando.Dto.RolId)
            {
                visualizacionBarrera.Rol = Repositorio.Obtener<Rol>(comando.Dto.RolId);
            }

            if (comando.SensoresBorrados != null)
            {
                foreach (var idBorrado in comando.SensoresBorrados)
                {
                    Repositorio.Remover<SensorBarrera>(idBorrado);
                }
            }

            if (comando.Dto.SensoresBarreras != null)
            {
                var sensores = (from sensor in comando.Dto.SensoresBarreras
                                  where !sensor._destroy
                                  select Conversor.Convertir<SensorBarreraDto, SensorBarrera>(sensor)).ToList();
                foreach (var sensor in sensores)
                {
                    visualizacionBarrera.SensoresBarreras.Add(sensor);
                }
            }
        }

        protected override void Validar(ModificarVisualizacionBarrera comando, Resultado resultado)
        {

        }
    }
}
