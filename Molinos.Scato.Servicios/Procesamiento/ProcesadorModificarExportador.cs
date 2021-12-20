using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarExportador : ProcesadorModificar<ModificarExportador>
    {
        public ProcesadorModificarExportador(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarExportador comando)
        {
            var exportador = Repositorio.Obtener<Exportador>(comando.Dto.Id);
            if (comando.Dto.Almacen_Id != null && comando.Dto.Almacen_Id != 0)
            {
                exportador.Almacen = Repositorio.Obtener<Almacen>(comando.Dto.Almacen_Id);
            }
            else
            {
                exportador.Almacen = null;
            }
            Conversor.Convertir(comando.Dto, exportador);
        }

        protected override void Validar(ModificarExportador comando, Resultado resultado)
        {

        }
    }
}
