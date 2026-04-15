using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogValidacionAccesoStopRespuesta : ProcesadorCrear<CrearLogValidacionAccesoStopRespuesta, LogValidacionAccesoStopRespuesta>
    {
        public ProcesadorCrearLogValidacionAccesoStopRespuesta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override LogValidacionAccesoStopRespuesta CrearEntidad(CrearLogValidacionAccesoStopRespuesta comando)
        {
            return Conversor.Convertir<LogValidacionAccesoStopRespuestaDto, LogValidacionAccesoStopRespuesta>(comando.Dto);
        }

        protected override void Validar(CrearLogValidacionAccesoStopRespuesta comando, Resultado resultado)
        {
        }
    }
}
