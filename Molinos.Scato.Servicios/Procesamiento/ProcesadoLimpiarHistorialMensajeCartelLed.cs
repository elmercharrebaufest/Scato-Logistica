using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadoLimpiarHistorialMensajeCartelLed : ProcesadorComando<LimpiarHistorialMensajeCartelLed>
    {
        public ProcesadoLimpiarHistorialMensajeCartelLed(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LimpiarHistorialMensajeCartelLed comando)
        {
            var resultadoMensajeCartelLed = new ResultadoMensajeCartelLedReordenado();
            var listaMensajes = Repositorio.Listar<MensajeCartelLed>(x => x.Codigo == comando.Codigo).OrderBy(x => x.Orden).ToList();
            if(comando.LimpiarCamion)
                LimpiarCamionEnCartel(listaMensajes, comando);
            else
                LimpiarCalleEnCartel(listaMensajes, comando.CalleId);
            resultadoMensajeCartelLed.ListaDeMensajes = Conversor.ConvertirList<MensajeCartelLed, MensajeCartelLedDto>(listaMensajes).ToList();
            return resultadoMensajeCartelLed;
        }

        private void LimpiarCalleEnCartel(List<MensajeCartelLed> listaMensajes, int calleId)
        {
            var mensajeCartelLedEntity = listaMensajes.FirstOrDefault(q => q.HistorialMensajeCartelLed?.Calle?.Id == calleId);
            var mensajeCalleCircular = listaMensajes.FirstOrDefault(x => x.HistorialMensajeCartelLed?.Calle?.TipoCalle == Dominio.Enums.TipoCalle.Circular);

            if (mensajeCartelLedEntity != null && mensajeCartelLedEntity.HistorialMensajeCartelLed != null)
            {
                mensajeCartelLedEntity.HistorialMensajeCartelLed.Calle = null;
                mensajeCartelLedEntity.HistorialMensajeCartelLed.Mensaje = null;
                mensajeCartelLedEntity.HistorialMensajeCartelLed.FechaUltimaModificacion = null;
                ReordenarMensajes(listaMensajes, mensajeCalleCircular?.Orden);
            }
        }

        private void ReordenarMensajes(List<MensajeCartelLed> listaMensajes, int? slotCircular = null)
        {
            var historial = listaMensajes
                .Where(x => x.HistorialMensajeCartelLed != null)
                .Where(x => x.HistorialMensajeCartelLed?.FechaUltimaModificacion != null)
                .Where(x => slotCircular == null || x.Orden != slotCircular)
                .Select(x => x.HistorialMensajeCartelLed)
                .OrderBy(x => x.FechaUltimaModificacion).ToList();

            var listMensajesCant = (slotCircular == null) ? listaMensajes.Count() : listaMensajes.Count() - 1;

            for (int i = 0; i < listMensajesCant; i++)
            {
                var tieneDatos = (i < historial.Count());

                if (listaMensajes[i].HistorialMensajeCartelLed == null)
                    continue;

                listaMensajes[i].HistorialMensajeCartelLed.Calle = (tieneDatos) ? historial[i].Calle : null;
                listaMensajes[i].HistorialMensajeCartelLed.Mensaje = (tieneDatos) ? historial[i].Mensaje : null;
                listaMensajes[i].HistorialMensajeCartelLed.FechaUltimaModificacion = (tieneDatos) ? historial[i].FechaUltimaModificacion : null;
            }

            Repositorio.GuardarCambios();
        }

        private void LimpiarCamionEnCartel(List<MensajeCartelLed> listaMensajes, LimpiarHistorialMensajeCartelLed comando)
        {
            var mensajeCartel = listaMensajes.FirstOrDefault(x => x.HistorialMensajeCartelLed != null && x.HistorialMensajeCartelLed.Calle.Id == comando.CalleId && x.HistorialMensajeCartelLed.Recorrido.Id == comando.RecorridoId);
            if(mensajeCartel != null)
            {
                mensajeCartel.HistorialMensajeCartelLed.Calle = null;
                mensajeCartel.HistorialMensajeCartelLed.Mensaje = null;
                mensajeCartel.HistorialMensajeCartelLed.FechaUltimaModificacion = null;
                mensajeCartel.HistorialMensajeCartelLed.Recorrido = null;
                Repositorio.GuardarCambios();
            }
        }
    }
}