using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
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
        public ProcesadorInsertarSlotMensajeCartelLed(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(InsertarSlotMensajeCartelLed comando)
        {
            var resultado = new ResultadoMensajeCartelLed();
            if (!Validar(comando))
            {
                resultado.Error("CartelLedLlamado", "La calle ya fue llamada");
                return resultado;
            }
            
            var listaMensajes = Repositorio.Listar<MensajeCartelLed>(x => x.Codigo == comando.Codigo).OrderBy(x => x.Orden).ToList();
            CrearHistorialMensajeCartelLedSiNoTiene(listaMensajes);
            var mensajeCartelLedEntity = new MensajeCartelLed();

            if (!comando.EsCircular && comando.OrdenCircular != null) // PreCalado
            {
                mensajeCartelLedEntity = InsertarSlotCartel(listaMensajes, comando.OrdenCircular);
            } else if (comando.EsCircular && comando.OrdenCircular != null) // Circular
            {
                mensajeCartelLedEntity = InsertarSlotCartelCircular(listaMensajes, comando.OrdenCircular.Value);
            } else if (comando.EsPrioritarioPrebalanza)
            {
                mensajeCartelLedEntity = InsertarSlotCartelPrebalanzaPrioritario(listaMensajes);
            }
            else // PreBalanza, PostCalado
            {
                mensajeCartelLedEntity = InsertarSlotCartel(listaMensajes);
            }

            if(mensajeCartelLedEntity != null)
            {
                var calle = Repositorio.Obtener<Calle>(x => x.Id == comando.CalleId);
                if(calle != null)
                {
                    mensajeCartelLedEntity.HistorialMensajeCartelLed.Calle = calle;
                    mensajeCartelLedEntity.HistorialMensajeCartelLed.Mensaje = calle.Nombre;
                    resultado.Mensaje = calle.Nombre;
                }
                mensajeCartelLedEntity.HistorialMensajeCartelLed.FechaUltimaModificacion = DateTime.Now;
                resultado.NumeroPrograma = mensajeCartelLedEntity.Programa;
                resultado.NumeroTrama = mensajeCartelLedEntity.Trama;
                resultado.NumeroVariable = mensajeCartelLedEntity.Variable;
                resultado.SegundosDeEspera = mensajeCartelLedEntity.SegundosDeEspera;
                Repositorio.GuardarCambios();
            }

            if(comando.EsPrioritarioPrebalanza)
                resultado.ListaDeMensajes = Conversor.ConvertirList<MensajeCartelLed, MensajeCartelLedDto>(listaMensajes).ToList();

            return resultado;
        }

        private bool Validar(InsertarSlotMensajeCartelLed comando)
        {
            bool valido = true;

            if(Repositorio.Existe<HistorialMensajeCartelLed>(x => x.Calle.Id == comando.CalleId))
            {
               valido = false;
            }

            return valido;
        }

        private void CrearHistorialMensajeCartelLedSiNoTiene(List<MensajeCartelLed> listaMensajes)
        {
            foreach (var mensajeCartelLed in listaMensajes)
            {
                if(mensajeCartelLed.HistorialMensajeCartelLed == null)
                {
                    mensajeCartelLed.HistorialMensajeCartelLed = new HistorialMensajeCartelLed()
                    {
                        Id = mensajeCartelLed.Id
                    };
                }
            }
        }

        private MensajeCartelLed InsertarSlotCartel(List<MensajeCartelLed> listaMensajes, int? slotCircular = null)
        {
            return listaMensajes.FirstOrDefault(q => q.HistorialMensajeCartelLed.FechaUltimaModificacion == null
            && (slotCircular == null || q.Orden != slotCircular));
        }

        private MensajeCartelLed InsertarSlotCartelCircular(List<MensajeCartelLed> listaMensajes, int slotCircular)
        {
           return listaMensajes.FirstOrDefault(q => q.Orden == slotCircular);
        }

        private MensajeCartelLed InsertarSlotCartelPrebalanzaPrioritario(List<MensajeCartelLed> listaMensajes)
        {
            ReordenarFilasPrebalanza(listaMensajes);
            return listaMensajes.FirstOrDefault(x => x.Variable == Constantes.ConfiguracionGeneral.PreBalanza.VariablePredeterminadaPasoPrioritaria);
        }

        private void ReordenarFilasPrebalanza(List<MensajeCartelLed> listaMensajes)
        {
            listaMensajes = listaMensajes.OrderBy(x => x.Variable).ToList();
            int indexVariablePasoDirecto = listaMensajes.Select(x => x.Variable).ToList().IndexOf(Constantes.ConfiguracionGeneral.PreBalanza.VariablePredeterminadaPasoPrioritaria);
            for (int i = listaMensajes.Count - 1; i > indexVariablePasoDirecto; i--)
            {
                listaMensajes[i].HistorialMensajeCartelLed.Calle = listaMensajes[i - 1].HistorialMensajeCartelLed.Calle;
                listaMensajes[i].HistorialMensajeCartelLed.Mensaje = listaMensajes[i - 1].HistorialMensajeCartelLed.Mensaje;
                listaMensajes[i].HistorialMensajeCartelLed.FechaUltimaModificacion = listaMensajes[i - 1].HistorialMensajeCartelLed.FechaUltimaModificacion;
            }

            listaMensajes[indexVariablePasoDirecto].HistorialMensajeCartelLed.Calle = null;
            listaMensajes[indexVariablePasoDirecto].HistorialMensajeCartelLed.Mensaje = null;
            listaMensajes[indexVariablePasoDirecto].HistorialMensajeCartelLed.FechaUltimaModificacion = null;
        }
    }
}