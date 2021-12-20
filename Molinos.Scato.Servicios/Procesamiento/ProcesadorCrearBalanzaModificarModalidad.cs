using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBalanzaModificarModalidad : ProcesadorCrear<CrearBalanzaModificarModalidad, BalanzaModificacionModalidad>
    {
        public ProcesadorCrearBalanzaModificarModalidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override BalanzaModificacionModalidad CrearEntidad(CrearBalanzaModificarModalidad comando)
        {
            var cambioDeModalidad = Conversor.Convertir<BalanzaModificacionModalidadDto, BalanzaModificacionModalidad>(comando.Dto);
            cambioDeModalidad.Balanza = Repositorio.Obtener<Balanza>(comando.Dto.BalanzaId);
            cambioDeModalidad.Balanza.Modalidad = comando.Dto.Modalidad;
            cambioDeModalidad.Balanza.EstaEnCero = false;
            return cambioDeModalidad;
        }

        protected override void Validar(CrearBalanzaModificarModalidad comando, Resultado resultado)
        {
        }
    }
}
