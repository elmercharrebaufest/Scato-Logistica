using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.Consultas;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarTicketsNoGranos : ProcesadorComando<ConsultarTicketsNoGranos>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorConsultarTicketsNoGranos(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }        

        public override Resultado Ejecutar(ConsultarTicketsNoGranos comando)
        {
            var resultado = new ResultadoConsultarTicketsNoGranos();
            try
            {
                var fechaDesde = comando.FechaDesde.Date;
                var fechaHasta = comando.FechaHasta.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                var recorridos = Repositorio.Listar<Recorrido>(x => x.Patente == comando.Patente && x.FechaInicio >= fechaDesde && x.FechaInicio <= fechaHasta).OrderByDescending(x => x.FechaInicio);
                if (!recorridos.Any())
                    throw new ConsultarTicketsNoGranosException("No se encontraron recorridos para la patente y fechas indicadas");

                foreach (var recorrido in recorridos)
                {
                    var resultadoTicketsNoGranos = new ResultadoTicketsNoGranos();
                    resultadoTicketsNoGranos.TicketPesada = ObtenerTicketPesada(recorrido.InstanciaWorkflow, recorrido.Centro.Id);
                    resultadoTicketsNoGranos.TicketReciboMunicipal = ObtenerTicketReciboMunicipal(recorrido.InstanciaWorkflow, recorrido.Centro.Id);
                    resultadoTicketsNoGranos.CPEDG = ObtenerCartaPorteDerivadoGranario(recorrido.Id);
                    resultadoTicketsNoGranos.NumeroOrdenOperaciones = ObtenerNumeroOperaciones(recorrido.Id, recorrido.TipoDocumentoIngreso);
                    resultado.TicketsNoGranos.Add(resultadoTicketsNoGranos);
                }
            }
            catch (ConsultarTicketsNoGranosException ex)
            {
                resultado.Error(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error($"Error ObtenerTicketsNoGranos: {ex.Message}");
                resultado.Error(string.Empty, "Ocurrió un error al obtener tickets no granos.");
            }

            return resultado;
        }

        public byte[] ObtenerTicketPesada(Guid workflowInstanceId, int centroId)
        {
            var idTicketPesada = Repositorio.ObtenerProyeccion<Impresion, int>(x => x.TipoImpresion == TipoImpresion.TicketPesada && x.WorkflowId == workflowInstanceId, x => x.Id);
            if (idTicketPesada == 0) return null;

            var preTicketPesada = servicioComandos.Ejecutar(new ImprimirDocumento
            {
                Id = idTicketPesada,
                Impresora = 0,
                CentroId = centroId,
                CantCopias = 1
            }) as ResultadoPrevisualizar;
            if (preTicketPesada == null || preTicketPesada.HayErrores)
                throw new ConsultarTicketsNoGranosException($"Ocurrió un error al obtener Ticket Pesada del recorrido {workflowInstanceId} y centro {centroId}");

            return preTicketPesada.Archivo;
        }

        public byte[] ObtenerTicketReciboMunicipal(Guid workflowInstanceId, int centroId)
        {
            var idReciboMunicipal = Repositorio.ObtenerProyeccion<Impresion, int>(x => x.TipoImpresion == TipoImpresion.ReciboMunicipal && x.WorkflowId == workflowInstanceId, x => x.Id);
            if (idReciboMunicipal == 0) return null;

            var preReciboMunicipal = servicioComandos.Ejecutar(new ImprimirDocumento
            {
                Id = idReciboMunicipal,
                Impresora = 0,
                CentroId = centroId,
                CantCopias = 1
            }) as ResultadoPrevisualizar;
            if (preReciboMunicipal == null || preReciboMunicipal.HayErrores)
                throw new ConsultarTicketsNoGranosException($"Ocurrió un error al obtener Recibo Municipal del recorrido {workflowInstanceId} y centro {centroId}");

            return preReciboMunicipal.Archivo;
        }

        public byte[] ObtenerCartaPorteDerivadoGranario(int recorridoId)
        {
            var cartaPorteDG = Repositorio.Obtener<CartaPorteDerivadoGranario>(x => x.Recorrido.Id == recorridoId);
            if (cartaPorteDG == null) return null;

            if (string.IsNullOrEmpty(cartaPorteDG.RutaFotoCPEDG) || !File.Exists(cartaPorteDG.RutaFotoCPEDG))
                throw new ConsultarTicketsNoGranosException($"No se pudo acceder a la Carta de Porte Derivado Granario para el recorrido {recorridoId}");
            
            byte[] pdf = File.ReadAllBytes(cartaPorteDG.RutaFotoCPEDG);
            return pdf;
        }

        public string ObtenerNumeroOperaciones(int recorridoId, TipoDocumentoIngreso tipoDocumento)
        {
            switch (tipoDocumento)
            {
                case TipoDocumentoIngreso.OrdenCargaInterna:
                    return Repositorio.ObtenerProyeccion<OrdenCargaInterna, string>(x => x.Recorrido.Id == recorridoId, x => x.Id_operaciones);
                case TipoDocumentoIngreso.OrdenCargaInternaFason:
                    return Repositorio.ObtenerProyeccion<OrdenCargaInternaFason, string>(x => x.Recorrido.Id == recorridoId, x => x.NumeroOrdenExterno);
                default:
                    return null;
            };
        }
    }

    public class ConsultarTicketsNoGranosException : Exception
    {
        public ConsultarTicketsNoGranosException(string message) : base(message)
        {
        }
    }
}
