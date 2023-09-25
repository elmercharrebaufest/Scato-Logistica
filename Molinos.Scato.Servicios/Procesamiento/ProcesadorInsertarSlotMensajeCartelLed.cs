using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorInsertarSlotMensajeCartelLed : ProcesadorComando<InsertarSlotMensajeCartelLed>
    {
        private List<string> codigosPreBalanza;

        public ProcesadorInsertarSlotMensajeCartelLed(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
            this.codigosPreBalanza = new List<string> {
                CodigoMensajeCartelLed.LlamadoCamionPreBalanza,
                CodigoMensajeCartelLed.LlamadoCallePreBalanza,
            };
        }

        public override Resultado Ejecutar(InsertarSlotMensajeCartelLed comando)
        {
            var resultado = new ResultadoMensajeCartelLed();
            if (Validar(comando, resultado))
                return resultado;

            var listaMensajes = Repositorio.Listar<MensajeCartelLed>(x => x.Codigo == comando.Codigo).OrderBy(x => x.Orden).ToList();
            CrearHistorialMensajeCartelLedSiNoTiene(listaMensajes);
            var mensajeCartelLedEntity = new MensajeCartelLed();

            if (comando.EsLlamadoPorCamion)
                mensajeCartelLedEntity = ObtenerSlotCartelParaCamion(comando, listaMensajes);
            else if (!comando.EsCircular && comando.OrdenCircular != null)
                mensajeCartelLedEntity = ObtenerSlotCartelParaCalle(listaMensajes, comando.OrdenCircular);
            else if (comando.EsCircular && comando.OrdenCircular != null)
                mensajeCartelLedEntity = ObtenerSlotCircularCartelParaCalle(listaMensajes, comando.OrdenCircular.Value);
            else
                mensajeCartelLedEntity = ObtenerSlotCartelParaCalle(listaMensajes);

            if (mensajeCartelLedEntity != null)
            {
                var calle = Repositorio.Obtener<Calle>(x => x.Id == comando.CalleId);
                mensajeCartelLedEntity.HistorialMensajeCartelLed.Calle = calle;
                mensajeCartelLedEntity.HistorialMensajeCartelLed.Mensaje = comando.EsLlamadoPorCamion ? comando.Patente : calle?.Nombre;
                mensajeCartelLedEntity.HistorialMensajeCartelLed.FechaUltimaModificacion = DateTime.Now;
                if (comando.EsLlamadoPorCamion)
                    mensajeCartelLedEntity.HistorialMensajeCartelLed.Recorrido = Repositorio.Obtener<Recorrido>(x => x.Id == comando.RecorridoId);

                resultado.Mensaje = mensajeCartelLedEntity.HistorialMensajeCartelLed.Mensaje;
                resultado.NumeroPrograma = mensajeCartelLedEntity.Programa;
                resultado.NumeroTrama = mensajeCartelLedEntity.Trama;
                resultado.NumeroVariable = mensajeCartelLedEntity.Variable;
                resultado.SegundosDeEspera = mensajeCartelLedEntity.SegundosDeEspera;
                Repositorio.GuardarCambios();
            }

            if (codigosPreBalanza.Contains(comando.Codigo))
            {
                var nuevosMensajes = Repositorio.Listar<MensajeCartelLed>(x => x.Codigo == comando.Codigo);
                resultado.ListaDeMensajes = Conversor.ConvertirList<MensajeCartelLed, MensajeCartelLedDto>(nuevosMensajes).ToList();
            }
            return resultado;
        }

        private MensajeCartelLed ObtenerSlotCartelParaCamion(InsertarSlotMensajeCartelLed comando, List<MensajeCartelLed> listaMensajes)
        {
            return comando.EsCamionEnEspera ? ObtenerSlotCamionEnEspera(comando, listaMensajes) : ObtenerSlotCamionLlamado(comando, listaMensajes);
        }

        private MensajeCartelLed ObtenerSlotCamionLlamado(InsertarSlotMensajeCartelLed comando, List<MensajeCartelLed> listaMensajes)
        {
            var mensajeCartelLed = listaMensajes.FirstOrDefault(x => x.HistorialMensajeCartelLed.Calle?.Id == comando.CalleId);
            return mensajeCartelLed ?? listaMensajes.FirstOrDefault(q => q.HistorialMensajeCartelLed.FechaUltimaModificacion == null);
        }

        private MensajeCartelLed ObtenerSlotCamionEnEspera(InsertarSlotMensajeCartelLed comando, List<MensajeCartelLed> listaMensajes)
        {
            var mensajeSlotCamionLlamado = Repositorio.Obtener<MensajeCartelLed>(x => x.Codigo == CodigoMensajeCartelLed.LlamadoCallePreBalanza && x.HistorialMensajeCartelLed.Calle.Id == comando.CalleId);
            return mensajeSlotCamionLlamado != null
                    ? listaMensajes.FirstOrDefault(x => x.Orden == mensajeSlotCamionLlamado.Orden)
                    : null;
        }

        private bool Validar(InsertarSlotMensajeCartelLed comando, ResultadoMensajeCartelLed resultado)
        {
            if (!comando.EsLlamadoPorCamion && Repositorio.Existe<HistorialMensajeCartelLed>(x => x.Calle.Id == comando.CalleId))
                resultado.Error("CartelLedLlamado", "La calle ya fue llamada");

            return resultado.HayErrores;
        }

        private void CrearHistorialMensajeCartelLedSiNoTiene(List<MensajeCartelLed> listaMensajes)
        {
            foreach (var mensajeCartelLed in listaMensajes)
            {
                if (mensajeCartelLed.HistorialMensajeCartelLed == null)
                {
                    mensajeCartelLed.HistorialMensajeCartelLed = new HistorialMensajeCartelLed()
                    {
                        Id = mensajeCartelLed.Id
                    };
                }
            }
        }

        private MensajeCartelLed ObtenerSlotCartelParaCalle(List<MensajeCartelLed> listaMensajes, int? slotCircular = null)
        {
            return listaMensajes.FirstOrDefault(q => q.HistorialMensajeCartelLed.FechaUltimaModificacion == null
            && (slotCircular == null || q.Orden != slotCircular));
        }

        private MensajeCartelLed ObtenerSlotCircularCartelParaCalle(List<MensajeCartelLed> listaMensajes, int slotCircular)
        {
            return listaMensajes.FirstOrDefault(q => q.Orden == slotCircular);
        }
    }
}