using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarLlamadoAutomaticoHidraulica : ProcesadorComando<ActualizarLlamadoAutomaticoHidraulica>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorActualizarLlamadoAutomaticoHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
            this.servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(ActualizarLlamadoAutomaticoHidraulica comando)
        {
            var resultado = new Resultado();
            var hidraulica = Repositorio.Obtener<LlamadoAutomaticoHidraulica>(q => q.Hidraulica.Id == comando.Id && (q.Estado != EstadoHidraulica.Inhabilitado || comando.Estado == EstadoHidraulica.Disponible));
            if(hidraulica != null)
            {
                hidraulica.Estado = comando.Estado;
                hidraulica.UltimaPatenteLlamada = comando.Patente;
                hidraulica.FechaUltimaModificacionEstado = DateTime.Now;
                if (comando.Estado == EstadoHidraulica.Disponible)
                {
                    LlamadoAutomaticoVolcadora(hidraulica);
                }
                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        private void LlamadoAutomaticoVolcadora(LlamadoAutomaticoHidraulica hidraulica)
        {
            var callesHidraulicas = Repositorio.Listar<ConfiguracionCalleHidraulica>();
            var primerosCamiones = new List<CamionHidraulicaDto>();
            foreach (var calleHidraulica in callesHidraulicas)
            {
                var resultado = servicioOrquestador.Ejecutar(
                    new EjecutarTomarFoto
                    {
                        CodigoDispositivo = calleHidraulica.CodigoCamaraALPR,
                        FilePath = string.Empty,
                        SubPath = string.Empty,
                        FileName = string.Empty
                    }) as ResultadoObtenerPatente;
                if (resultado == null || string.IsNullOrEmpty(resultado.Patente))
                    continue;

                var hidraulicas = Repositorio.Listar<LlamadoAutomaticoHidraulica>(x => x.UltimaPatenteLlamada == resultado.Patente);
                if (hidraulicas.Any())
                    continue;

                var datosCamion = ObtenerDatosPorPatente(resultado.Patente);
                if (datosCamion == null)
                    continue;

                var callePorRecorrido = Repositorio.Obtener<CallePorRecorrido>(x => x.Recorrido.Id == datosCamion.RecorridoId && x.FechaEgreso == null && x.Calle.TipoCalle == TipoCalle.PlayaInterna && !x.Recorrido.Terminado);
                if (callePorRecorrido == null)
                    continue;

                if (!datosCamion.HidraulicasId.Contains(hidraulica.Hidraulica.Id))
                    continue;

                datosCamion.FechaLlegadaACalleHidraulica = callePorRecorrido.FechaIngeso;
                datosCamion.CodigoCartel = calleHidraulica.CodigoCartel;
                primerosCamiones.Add(datosCamion);
            }
            if (primerosCamiones.Count > 0)
            {
                var camionLlamado = primerosCamiones.OrderBy(x => x.FechaLlegadaACalleHidraulica).FirstOrDefault();
                EnviarMensajeACartel(camionLlamado.CodigoCartel, $"{camionLlamado.Patente} avance a {hidraulica.Hidraulica.Nombre}");
                hidraulica.Estado = EstadoHidraulica.Llamando;
                hidraulica.UltimaPatenteLlamada = camionLlamado.Patente;
            }
        }

        private void EnviarMensajeACartel(string codigoCartel, string mensaje)
        {
            try
            {
                var mensajeCartel = servicioRepositorio.ObtenerMensajeCartelLedPorCodigo(CodigoMensajeCartelLed.LlamadoAutomaticoVolcadoras);
                if (!string.IsNullOrEmpty(codigoCartel) && mensaje != null)
                {
                    servicioComandos.Ejecutar(new EnviarMensajeCarteLed
                    {
                        Mensaje = mensaje,
                        Codigo = codigoCartel,
                        NumeroPrograma = mensajeCartel.Programa,
                        NumeroTrama = mensajeCartel.Trama,
                        NumeroVariable = mensajeCartel.Variable,
                        SegundosDeEspera = mensajeCartel.SegundosDeEspera
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo mostrar el mensaje en Cartel Led");
            }
        }

        private CamionHidraulicaDto ObtenerDatosPorPatente(string patente)
        {
            CamionHidraulicaDto datosCamion = null;
            try
            {
                var recorrido = servicioRepositorio.ObtenerRecorridoActivoPorPatente(patente);
                if (recorrido != null)
                {
                    datosCamion = new CamionHidraulicaDto()
                    {
                        Patente = patente,
                        HidraulicasId = recorrido.HidraulicasId,
                        RecorridoId = recorrido.Id
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se obtener datos por patente {0}", patente);
            }
            return datosCamion;
        }
    }
}