using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorObtenerTickets : ProcesadorComando<ObtenerTickets>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicioRepositorio;

        public ProcesadorObtenerTickets(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio) : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioRepositorio = servicioRepositorio;
        }        

        public override Resultado Ejecutar(ObtenerTickets comando)
        {
            Log.Debug($"Obteniendo datos de la cp {comando.CP}");
            var resultado = new ResultadoTickets();
            var recorrido = Repositorio.ObtenerMayor<Recorrido, int>(x => x.NumeroDocumentoIngreso == comando.CP, x=>x.Id);
            Log.Debug($"Recorrido Obtenido {recorrido.InstanciaWorkflow}");
            resultado.FotoCP = servicioRepositorio.ObtenerFotosCartaPortePorNumero(comando.CP);
            if (recorrido != null)
            {
                resultado.Patente = recorrido.Patente;
                resultado.CP = recorrido.NumeroDocumentoIngreso;
                try
                {
                    var idReciboMunicipal = Repositorio.Listar<Impresion, int>(x => x.Id, x =>
                      x.TipoImpresion == TipoImpresion.ReciboMunicipal && x.WorkflowId == recorrido.InstanciaWorkflow)
                        .OrderByDescending(x => x).FirstOrDefault();
                    Log.Debug($"Recibo municipal obtenido {idReciboMunicipal}");
                    var preReciboMunicipal = servicioComandos.Ejecutar(new ImprimirDocumento
                    {
                        Id = idReciboMunicipal,
                        Impresora = 0,
                        CentroId = recorrido.Centro.Id,
                        CantCopias = 1
                    });
                    if (!preReciboMunicipal.HayErrores)
                    {
                        resultado.TicketReciboMunicipal = ((ResultadoPrevisualizar)preReciboMunicipal).Archivo;
                    }
                }
                catch (Exception e)
                {
                    resultado.Error("ReciboMunicipal", e.Message);
                    Log.Error($"Error imprimir recibo municipal: {e.Message}");
                }
                try
                {
                    var idTicketPesada = Repositorio.Listar<Impresion, int>(x => x.Id, x =>
                      x.TipoImpresion == TipoImpresion.TicketPesada && x.WorkflowId == recorrido.InstanciaWorkflow)
                        .OrderByDescending(x => x).FirstOrDefault();
                    Log.Debug($"Ticket pesada obtenido {idTicketPesada}");

                    var preTicketPesada = servicioComandos.Ejecutar(new ImprimirDocumento
                    {
                        Id = idTicketPesada,
                        Impresora = 0,
                        CentroId = recorrido.Centro.Id,
                        CantCopias = 1
                    });
                    if (!preTicketPesada.HayErrores)
                    {
                        resultado.TicketPesada = ((ResultadoPrevisualizar)preTicketPesada).Archivo;
                    }
                }
                catch (Exception e)
                {
                    resultado.Error("TicketPesada", e.Message);
                    Log.Error($"Error ticket pesada municipal: {e.Message}");
                }
                try
                {
                    var idCertificadoCP = Repositorio.Listar<Impresion, int>(x => x.Id, x =>
                        x.TipoImpresion == TipoImpresion.CertificadoDeCartaPorte && x.WorkflowId == recorrido.InstanciaWorkflow)
                        .OrderByDescending(x => x).FirstOrDefault();
                    Log.Debug($"Certificado de Carta de Porte obtenido {idCertificadoCP}");

                    var preCertificadoCP = servicioComandos.Ejecutar(new ImprimirDocumento
                    {
                        Id = idCertificadoCP,
                        Impresora = 0,
                        CentroId = recorrido.Centro.Id,
                        CantCopias = 1
                    });
                    if(!preCertificadoCP.HayErrores)
                    {
                        resultado.CertificadoCP = ((ResultadoPrevisualizar)preCertificadoCP).Archivo;
                    }
                    
                }
                catch (Exception e)
                {
                    resultado.Error("Certificado Carta de Prote", e.Message);
                    Log.Error($"Error Certificado Carta de Porte: {e.Message}");
                }
            }
            return resultado;
        }
    }
}
