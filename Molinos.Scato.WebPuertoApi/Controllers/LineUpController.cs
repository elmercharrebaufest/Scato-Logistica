using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebPuertoApi.Atributos;
using Molinos.Scato.WebPuertoApi.EXCEL;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Molinos.Scato.WebPuertoApi.Controllers
{
    public class LineUpController : BaseController
    {
        private readonly IServicioActividadFactory<ILineUpService> factory;
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioComandos servicioComandos;

        public LineUpController(IServicioActividadFactory<ILineUpService> factory, 
            IServicioRepositorio servicio, 
            IListaDeWorkflows workflows,
            IServicioComandos servicioComandos) : base(servicio)
        {
            this.factory = factory;
            this.workflows = workflows;
            this.servicioComandos = servicioComandos;
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/LineUp/ResetearAgencia")]
        public HttpResponseMessage ResetearAgencia()
        {
            try
            {
                var embarques = workflows.ListarEmbarques();
                foreach(var embarque in embarques.Where(x => x.LineUp != null && x.LineUp.AgenciaContactada))
                {
                    embarque.LineUp.AgenciaContactada = false;
                    this.Modificar(embarque.LineUp);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //[HttpGet]
        //[Autorizacion(PermisosScato.LineUp)]
        //[Route("api/LineUp/ActualizarEstadoPuerto")]
        //public HttpResponseMessage ActualizarEstadoPuerto()
        //{
        //    try
        //    {
        //        servicioComandos.Ejecutar(new ActualizarEstadoPuerto());
        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        [HttpGet]
        [Autorizacion(PermisosScato.LineUpLectura)]
        [Route("api/LineUp/ObtenerEstadoPuerto")]
        public HttpResponseMessage ObtenerEstadoPuerto()
        {
            try
            {
                var estado = servicio.ObtenerEstadoPuerto();
                return Request.CreateResponse(HttpStatusCode.OK, estado);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }            
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/LineUp/Modificar")]
        public HttpResponseMessage Modificar(LineUpDto lineUp)
        {
            //TODO: asignar codigo de workflow correcto
            var workflow = "WorkflowPuerto";
            var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
            var servicioWf = factory.CrearServicio(workflowDefinicionId);

            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActLineUp,
                ActividadXaml = "LineUp",
                NombreUsuario = nombreUsuario,
                WorkflowInstanceId = lineUp.InstanciaWorkflow
            };

            try
            {
                if (lineUp.Ubicacion == 1) /**Zarpó**/
                    lineUp.PlanoDeCargaEnviado = true;
                var resultado = servicioWf.LineUp(controlRecorrido, lineUp, lineUp.InstanciaWorkflow) as Dominio.Comandos.ResultadoCrearWorkflow;
                if (resultado.HayErrores)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, resultado.Mensaje);
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,e.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUpExportar)]
        [Route("api/LineUp/ExportarEmbarques")]
        public HttpResponseMessage ExportarEmbarques()
        {
            var docFile = "Line Up " + DateTime.Now.ToString("yyyy-MM-dd");
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                var embarques = workflows.ListarEmbarques();
                var estado = servicio.ObtenerEstadoPuerto();
                var resultado = new ResultadoPrevisualizar();
                var generadorExcel = new ExcelLineUp();
                generadorExcel.GenerarArchivo(resultado, embarques, estado);

                response.Content = new ByteArrayContent(resultado.Archivo);
                response.Content.Headers.ContentLength = resultado.Archivo.LongLength;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ReasonPhrase = string.Format("File not found: {0} .", docFile);
                throw new HttpResponseException(response);
            }
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = docFile + ".xls";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(docFile + ".xls"));
            return response;
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUpExportar)]
        [Route("api/LineUp/EnviarPorMail")]
        public void EnviarPorMail(MailDto mail)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            var docFile = "Line Up " + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
            try
            {
                var embarques = workflows.ListarEmbarques();
                var estado = servicio.ObtenerEstadoPuerto();
                var resultado = new ResultadoPrevisualizar();
                var generadorExcel = new ExcelLineUp();
                generadorExcel.GenerarArchivo(resultado, embarques, estado);

                if (!HttpContext.Current.Request.IsLocal)
                    mail.Destinatarios.Add("scatoprodMOA@molinosagro.com.ar");

                // CARACTERES NO IMPRIMIBLES:
                // Enter: (\n -> <br/>)
                // Tabulador: (\t -> &nbsp;&nbsp;&nbsp;&nbsp;)
                // Negrita: (\f -> <b>) (\f\f -> </b>)
                // Subrayado: (\0 -> <u>) (\0\0 -> </u>)
                servicioComandos.Ejecutar(new EnvioMail
                {
                    Cuerpo = mail.Body.Replace("\n", "<br/>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;")
                        .Replace("\f\f", "</b>").Replace("\f", "<b>").Replace("\0\0", "</u>").Replace("\0", "<u>"),
                    Destinatarios = mail.Destinatarios,
                    Titulo= $"Line Up {DateTime.Now:dd-MM-yyyy HH:mm}",
                    Attachment = resultado.Archivo,
                    AttachmentName = docFile
                });
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ReasonPhrase = string.Format("File not found: {0} .", docFile);
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUpExportar)]
        [Route("api/LineUp/ObtenerDestinatariosLineUp")]
        public HttpResponseMessage ObtenerDestinatariosLineUp()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ObtenerUsuariosLineUp()
            );
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/LineUp/ModificarEstadosPuerto")]
        public HttpResponseMessage ModificarEstadosPuerto(EstadoPuertoDto estadoPuerto)
        {
            servicioComandos.Ejecutar(new CrearEstadoPuerto { Dto = estadoPuerto });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}