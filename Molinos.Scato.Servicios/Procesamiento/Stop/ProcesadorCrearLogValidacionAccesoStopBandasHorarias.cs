using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogValidacionAccesoStopBandasHorarias : ProcesadorCrear<CrearLogValidacionAccesoStopBandasHorarias, LogValidacionAccesoStopBandasHorarias>
    {
        public ProcesadorCrearLogValidacionAccesoStopBandasHorarias(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override LogValidacionAccesoStopBandasHorarias CrearEntidad(CrearLogValidacionAccesoStopBandasHorarias comando)
        {
            return Conversor.Convertir<LogValidacionAccesoStopBandasHorariasDto, LogValidacionAccesoStopBandasHorarias>(comando.Dto);
        }

        protected override void Validar(CrearLogValidacionAccesoStopBandasHorarias comando, Resultado resultado)
        {
        }
    }
}
