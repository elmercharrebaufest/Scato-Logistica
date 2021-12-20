using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearNirsModificarModalidad : ProcesadorCrear<CrearNirsModificarModalidad, NirsModificacionModalidad>
    {
        public ProcesadorCrearNirsModificarModalidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override NirsModificacionModalidad CrearEntidad(CrearNirsModificarModalidad comando)
        {
            var cambioDeModalidad = Conversor.Convertir<NirsModificacionModalidadDto, NirsModificacionModalidad>(comando.Dto);
            cambioDeModalidad.Nirs = Repositorio.Obtener<Nirs>(comando.Dto.NirsId);
            cambioDeModalidad.Nirs.Modalidad = comando.Dto.Modalidad;
            return cambioDeModalidad;
        }

        protected override void Validar(CrearNirsModificarModalidad comando, Resultado resultado)
        {
        }
    }
}
