using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadoLimpiarRecorridoHistorialMensajeCartelLed : ProcesadorComando<LimpiarRecorridoHistorialMensajeCartelLed>
    {
        public ProcesadoLimpiarRecorridoHistorialMensajeCartelLed(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LimpiarRecorridoHistorialMensajeCartelLed comando)
        {
            var resultado = new ResultadoMensajeCartelLed();
            var MensajEntity = Repositorio.Obtener<MensajeCartelLed>(x => x.HistorialMensajeCartelLed.Recorrido.Id == comando.RecorridoId && x.Codigo == comando.Codigo);

            if(MensajEntity != null)
            {
                MensajEntity.HistorialMensajeCartelLed.Calle = null;
                MensajEntity.HistorialMensajeCartelLed.Mensaje = null;
                MensajEntity.HistorialMensajeCartelLed.FechaUltimaModificacion = null;
                MensajEntity.HistorialMensajeCartelLed.Recorrido = null;

                Repositorio.GuardarCambios();               

                var result = Conversor.Convertir<MensajeCartelLed, MensajeCartelLedDto>(MensajEntity);

                resultado.NumeroPrograma = result.Programa;
                resultado.NumeroTrama = result.Trama;
                resultado.NumeroVariable = result.Variable;
                resultado.SegundosDeEspera = result.SegundosDeEspera;
                resultado.Mensaje = resultado.Mensaje;

            }




            return resultado;
        }
    }
}