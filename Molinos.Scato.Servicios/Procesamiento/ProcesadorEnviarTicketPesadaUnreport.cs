using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Urenport;
using Ninject.Extensions.Logging;
using PdfiumViewer;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnviarTicketPesadaUnreport : ProcesadorComando<EnviarTicketPesadaUnreport>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly calpesSoap urenport;
        private readonly IConfiguracionProvider configuracion;

        public ProcesadorEnviarTicketPesadaUnreport(IRepositorio repositorio, IServicioComandos servicioComandos, IConversor conversor, ILogger log, Urenport.calpesSoap urenport, IConfiguracionProvider configuracion)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.urenport = urenport;
            this.configuracion = configuracion;
        }

        public override Resultado Ejecutar(EnviarTicketPesadaUnreport comando)
        {
            var resultado = new Resultado();
            EnvioUrenport estadoUrenport = null;
            if (comando.EnvioUrenport > 0)
            {
                estadoUrenport = Repositorio.Obtener<EnvioUrenport>(x => x.Id == comando.EnvioUrenport);
            }
            else
            {
                var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.WorkflowId);
                estadoUrenport = Repositorio.Obtener<EnvioUrenport>(x => x.Recorrido.Id == recorrido.Id && x.TipoDoc == "2");
                if (estadoUrenport == null)
                {
                    estadoUrenport = Repositorio.Agregar(new EnvioUrenport
                    {
                        NumeroDocumentoIngreso = recorrido.NumeroDocumentoIngreso.TrimStart(new Char[] { '0' }),
                        TipoDoc = "2",
                        Ruta = "",
                        Recorrido = recorrido,
                        Estado = EstadoTransmisionASap.Pendiente,
                        Fecha = DateTime.Now
                    });
                }
            }



            try
            {
                comando.TicketPesadaId = Repositorio.ObtenerProyeccion<Dominio.Entidades.Impresion, int>(x => x.WorkflowId == comando.WorkflowId && x.TipoImpresion == TipoImpresion.ConstanciaDeEntregaLaser, x => x.Id);
                if (comando.TicketPesadaId == 0)
                {
                    var error = "No se encontró impresion del ticket de pesada id " + comando.TicketPesadaId + " para el circuito" + comando.WorkflowId;
                    Log.Error(error);
                    estadoUrenport.Estado = EstadoTransmisionASap.Error;
                    estadoUrenport.Error = error;
                    resultado.Errores.Add("ImpresionInexistente", error);
                }
                else
                {
                    Log.Debug("ProcesadorEnviarTicketPesadaUnreport - Inicio la consulta");

                    var response = urenport.Imagen(new ImagenRequest
                    {
                        Body = new ImagenRequestBody
                        {
                            Au = configuracion.AppSettings["UnreportAu"],
                            numCP = estadoUrenport.NumeroDocumentoIngreso,
                            tipo_doc = estadoUrenport.TipoDoc,
                            pic = ObtenerImagen(comando, estadoUrenport.Recorrido.Centro.Id)
                        }
                    });
                    Log.Debug("ProcesadorEnviarTicketPesadaUnreport - Realizo la consulta ");


                    if (!response.Body.ImagenResult.Contains("OK"))
                    {
                        resultado.Errores.Add("CodigoDeBaja", response.Body.ImagenResult);
                        Log.Error("ProcesadorEnviarTicketPesadaUnreport -" + response.Body.ImagenResult);
                        estadoUrenport.Error = response.Body.ImagenResult;
                        estadoUrenport.Estado = EstadoTransmisionASap.Error;
                    }
                    else
                    {
                        estadoUrenport.Estado = EstadoTransmisionASap.Correcto;
                        Log.Debug("ProcesadorEnviarTicketPesadaUnreport {0} procesada correctamente", estadoUrenport.NumeroDocumentoIngreso);
                    }
                }
            }
            catch (FaultException e)
            {
                Log.Error(e, "No se pudo registrar en urenport la CP {0}", estadoUrenport.NumeroDocumentoIngreso);
                resultado.Errores.Add("CodigoDeBaja", e.Message);
                estadoUrenport.Error = e.Message;
                estadoUrenport.Estado = EstadoTransmisionASap.Error;
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo registrar en urenport la CP {0}", estadoUrenport.NumeroDocumentoIngreso);
                resultado.Errores.Add("CodigoDeBaja", Textos.Error_Generico);
                estadoUrenport.Error = Textos.Error_Generico;
                estadoUrenport.Estado = EstadoTransmisionASap.Error;
            }

            Repositorio.GuardarCambios();
            return resultado;

        }


        private byte[] ObtenerImagen(EnviarTicketPesadaUnreport comando, int centro_id)
        {
            try
            {
                var resultado =
                    servicioComandos.Ejecutar(new ImprimirDocumento
                    {
                        Id = comando.TicketPesadaId,
                        Impresora = 0,
                        CentroId = centro_id,
                        CantCopias = 1
                    });
                if (!resultado.HayErrores)
                {

                    byte[] file = ((ResultadoPrevisualizar)resultado).Archivo;

                    try
                    {
                        
                        using (var document = PdfDocument.Load(new MemoryStream(file)))
                        {
                            var image = document.Render(0, 300, 300, true);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                return ms.ToArray();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Error al Convertir el Ticket de Pesada {comando.TicketPesadaId}");
                        return null;
                    }

                }
                return null;
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al Obetener el Ticket de Pesada {comando.TicketPesadaId}");
                return null;
            }
        }
    }
}