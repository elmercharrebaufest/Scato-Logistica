using iTextSharp.text;
using iTextSharp.text.pdf;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class ImpresionGenericaFile : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<string> NumeroDeDocumentoDeIngreso { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new ResultadoCrear();

            var workflowId = WorkflowId.Get<Guid>(context);
            var numeroDeDocumentoDeIngreso = NumeroDeDocumentoDeIngreso.Get<string>(context);            
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var codigo = CodigoDeImpresion.Get<string>(context);
            var centroId = CentroId.Get<int>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            try
            {
                var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Generica File",
                    ActividadXaml = "ImpresionGenericaFile",
                    WorkflowInstanceId = workflowId,
                    Fecha = DateTime.Now
                };
                resultado = servicio.Ejecutar(new CrearLogActividad { 
                    Dto = logActividad 
                }) as ResultadoCrear;
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }


            try
            {
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null)
                {
                    throw new Exception($"No existe el Documento de impresión {codigo}");
                }

                if (Enum.GetName(typeof(TipoImpresion), TipoImpresion.CartaDePorteElectronica) == documento.CodigoDocumentoImpresion)
                {
                    var recorrido = repositorio.ObtenerRecorridoPorGuid(workflowId);
                    var cartaporteElectronica = repositorio.ObtenerCartaPorteElectronicaPorCTG(recorrido?.NumeroDocumentoIngreso);

                    if(!(cartaporteElectronica is null))
                    {
                        ImprimirFileGenerico(servicio, cartaporteElectronica.Pdf, cantCopias, documento.ImpresoraDireccion, documento.CodigoDocumentoImpresion, workflowId, resultado);
                    }                    
                } else if (Enum.GetName(typeof(TipoImpresion), TipoImpresion.CartaPorteElectronicaDerivadoGranario) == documento.CodigoDocumentoImpresion)
                {
                    var cartaporteElectronica = repositorio.ObtenerCartaPorteDerivadoGranarioPorGuid(workflowId);
                    if (cartaporteElectronica != null && !string.IsNullOrEmpty(cartaporteElectronica.RutaFotoCPEDG) && File.Exists(cartaporteElectronica.RutaFotoCPEDG))
                    {
                        byte[] pdf = null;
                        var document = new Document();
                        using (var stream = new MemoryStream())
                        {
                            PdfWriter.GetInstance(document, stream);
                            document.Open();
                            Image image = Image.GetInstance(cartaporteElectronica.RutaFotoCPEDG);
                            image.ScaleAbsolute(document.PageSize.Width, document.PageSize.Height);
                            image.SetAbsolutePosition(0, 0);
                            document.Add(image);
                            document.Close();
                            pdf = stream.ToArray();
                        }
                        ImprimirFileGenerico(servicio, pdf, cantCopias, documento.ImpresoraDireccion, documento.CodigoDocumentoImpresion, workflowId, resultado);
                    }
                }              
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            if(resultado.HayErrores)
            {
                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "Impresion Generica File",
                        Fecha = DateTime.Now,
                        Comentario = resultado.Errores.Values.First(),
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionGenericaFile", PuestoDeTrabajoId = puestoDeTrabajoId }) as ResultadoCrear;
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }

        private void ImprimirFileGenerico(IServicioComandos servicioComando, byte[] pdf, int cantidadCopias, string impresora, string codigoDocumentoImpresion, Guid worfklowInstance, ResultadoCrear resultado)
        {
            if(!(pdf is null))
            {
                resultado = servicioComando.Ejecutar(new ImprimirFileGenerico 
                { 
                    CantidadCopias = cantidadCopias, 
                    File = pdf, 
                    Impresora = impresora ?? string.Empty, 
                    CodigoDocumentoImpresion = codigoDocumentoImpresion
                }) as ResultadoCrear;
            }

            if(pdf is null && ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
            {
                servicioComando.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "EnviarMensaje",
                        Fecha = DateTime.Now,
                        Comentario = "Error, no se encontro el PDF en la tabla CartaPorteElectronica.",
                        NombreUsuario = string.Empty,
                        WorkflowInstanceId = worfklowInstance,
                    }
                });
            }
        }
    }
}
