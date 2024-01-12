using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCallePreBalanza : ProcesadorModificar<ModificarCallePrebalanza>
    {
        public ProcesadorModificarCallePreBalanza(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCallePrebalanza comando)
        {
            var calle = Repositorio.Obtener<Calle>(comando.Dto.Id);
            calle.Nombre = comando.Dto.Descripcion;
            calle.CantidadDeCamiones = comando.Dto.Camiones;
        }

        protected override void Validar(ModificarCallePrebalanza comando, Resultado resultado)
        {
        }
    }
}