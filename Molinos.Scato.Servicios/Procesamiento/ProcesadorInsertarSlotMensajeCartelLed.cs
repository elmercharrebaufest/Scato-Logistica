using Molinos.Scato.Dominio.Comandos;
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
            if(!Validar(comando))
                return resultado;
            
            var listaMensajes = Repositorio.Listar<MensajeCartelLed>(x => x.Codigo == comando.Codigo).OrderBy(x => x.Orden).ToList();
            CrearHistorialMensajeCartelLedSiNoTiene(listaMensajes);
            var mensajeCartelLedEntity = new MensajeCartelLed();

            if(!comando.EsCircular && comando.OrdenCircular != null) // PreCalado
            {
                mensajeCartelLedEntity = InsertarSlotCartel(listaMensajes, comando.OrdenCircular);
            } else if(comando.EsCircular && comando.OrdenCircular != null) // Circular
            {
                mensajeCartelLedEntity = InsertarSlotCartelCircular(listaMensajes, comando.OrdenCircular.Value);
            } else // PreBalanza, PostCalado
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

            return resultado;
        }

        private bool Validar(InsertarSlotMensajeCartelLed comando)
        {
            if(Repositorio.Existe<HistorialMensajeCartelLed>(x => x.Calle.Id == comando.CalleId))
                return false;

            return true;
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

    }
}