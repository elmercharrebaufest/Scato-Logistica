using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebPuertoApi.Atributos;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Molinos.Scato.Servicios.Procesamiento;

namespace Molinos.Scato.WebPuertoApi.Controllers
{
    public class PlanoDeCargaController : BaseController
    {
        private readonly IServicioComandos comandos;

        public PlanoDeCargaController(IServicioActividadFactory<IIngresarEmbarqueService> factory, 
            IServicioRepositorio servicio, IServicioComandos comandos) : base(servicio)
        {
            this.comandos = comandos;
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ListarDestinos")]
        public HttpResponseMessage ListarDestinos()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarTodosDestinos()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ListarAgenciasControlPrivado")]
        public HttpResponseMessage ListarAgenciasControlPrivado()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarAgenciasControlPrivado()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ListarAgentesControlPrivado")]
        public HttpResponseMessage ListarAgentesControlPrivado()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarAgentesControlPrivado()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ListarEstibas")]
        public HttpResponseMessage ListarEstibas()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarEstibas()
            );
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/GuardarPlanoDeCarga")]
        public HttpResponseMessage GuardarPlanoDeCarga(PlanoDeCargaDto planoDeCarga)
        {
            comandos.Ejecutar(new GuardarPlanoDeCarga { Dto = planoDeCarga });
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ListarExportadores")]
        public HttpResponseMessage ListarExportadores()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListaExportadores()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ObtenerPlanoDeCarga")]
        public HttpResponseMessage ObtenerPlanoDeCarga(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ObtenerPlanoDeCarga(id)
            );
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/AgregarEstiba")]
        public HttpResponseMessage AgregarEstiba(EstibaDto estiba)
        {
            comandos.Ejecutar(new CrearEstiba { Dto = estiba });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ModificarEstiba")]
        public HttpResponseMessage ModificarEstiba(EstibaDto estiba)
        {
            comandos.Ejecutar(new ModificarEstiba { Dto = estiba });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/EliminarEstiba")]
        public HttpResponseMessage EliminarEstiba(int estibaId)
        {
            var resultado = comandos.Ejecutar(new EliminarEstiba { Id = estibaId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/AgregarAgenciaControlPrivado")]
        public HttpResponseMessage AgregarAgenciaControlPrivado(AgenciaControlPrivadoDto agencia)
        {
            comandos.Ejecutar(new CrearAgenciaControlPrivado { Dto = agencia });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ModificarAgenciaControlPrivado")]
        public HttpResponseMessage ModificarAgenciaControlPrivado(AgenciaControlPrivadoDto agencia)
        {
            comandos.Ejecutar(new ModificarAgenciaControlPrivado { Dto = agencia });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/EliminarAgenciaControlPrivado")]
        public HttpResponseMessage EliminarAgenciaControlPrivado(int agenciaId)
        {
            var resultado = comandos.Ejecutar(new EliminarAgenciaControlPrivado { Id = agenciaId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/AgregarAgenteControlPrivado")]
        public HttpResponseMessage AgregarAgenteControlPrivado(AgenteControlPrivadoDto agente)
        {
            comandos.Ejecutar(new CrearAgenteControlPrivado { Dto = agente });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/ModificarAgenteControlPrivado")]
        public HttpResponseMessage ModificarAgenteControlPrivado(AgenteControlPrivadoDto agente)
        {
            comandos.Ejecutar(new ModificarAgenteControlPrivado { Dto = agente });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/PlanoDeCarga/EliminarAgenteControlPrivado")]
        public HttpResponseMessage EliminarAgenteControlPrivado(int agenteId)
        {
            var resultado = comandos.Ejecutar(new EliminarAgenteControlPrivado { Id = agenteId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUpExportar)]
        [Route("api/PlanoDeCarga/ObtenerDestinatariosPlanoDeCarga")]
        public HttpResponseMessage ObtenerDestinatariosPlanoDeCarga()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ObtenerUsuariosPlanoDeCarga()
            );
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUpExportar)]
        [Route("api/PlanoDeCarga/ObtenerBodyPlanoDeCarga")]
        public HttpResponseMessage ObtenerBodyPlanoDeCarga([FromUri]int planoDeCargaId, [FromBody]EmbarqueDto embarque)
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ObtenerBodyPlanoDeCarga(planoDeCargaId, embarque)
            );
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUpExportar)]
        [Route("api/PlanoDeCarga/EnviarPorMail")]
        public void EnviarPorMail(int planoDeCargaId, MailDto mail)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            try
            {
                var plano = servicio.ObtenerPlanoDeCarga(planoDeCargaId);

                if (!HttpContext.Current.Request.IsLocal)
                    mail.Destinatarios.Add("scatoprodMOA@molinosagro.com.ar");

                // CARACTERES NO IMPRIMIBLES:
                // Enter: (\n -> <br/>)
                // Tabulador: (\t -> &nbsp;&nbsp;&nbsp;&nbsp;)
                // Negrita: (\f -> <b>) (\f\f -> </b>)
                // Subrayado: (\0 -> <u>) (\0\0 -> </u>)
                if (mail.Adjunto == null)
                {                
                    comandos.Ejecutar(new EnvioMail
                    {
                        Cuerpo = mail.Body.Replace("\n", "<br/>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;")
                            .Replace("\f\f", "</b>").Replace("\f", "<b>").Replace("\0\0", "</u>").Replace("\0", "<u>"),
                        Destinatarios = mail.Destinatarios,
                        Titulo = mail.Titulo,
                        Attachment = plano.FilePathPlano == null ? null : Convert.FromBase64String(plano.FilePathPlano),
                        AttachmentName = plano.PlanoDeCargaArchivoPlanoNombre,
                    
                        Attachment2 = plano.FilePathSecuencia == null ?
                            mail.Adjunto == null ? null : Convert.FromBase64String(mail.Adjunto) :
                            plano.FilePathSecuencia == null ? null : Convert.FromBase64String(plano.FilePathSecuencia),
                        AttachmentName2 = plano.FilePathSecuencia == null ?
                            mail.Nombre : plano.PlanoDeCargaArchivoSecuenciaNombre,
                    
                        Attachment3 = plano.FilePathSecuencia == null ?
                            null : mail.Adjunto == null ? null : Convert.FromBase64String(mail.Adjunto),
                        AttachmentName3 = plano.FilePathSecuencia == null ? null : mail.Nombre
                    });
                }
                else
                {
                    comandos.Ejecutar(new EnvioMail
                    {
                        Cuerpo = mail.Body.Replace("\n", "<br/>").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;")
                            .Replace("\f\f", "</b>").Replace("\f", "<b>").Replace("\0\0", "</u>").Replace("\0", "<u>"),
                        Destinatarios = mail.Destinatarios,
                        Titulo = mail.Titulo,
                        Attachment = mail.Adjunto == null ? null : Convert.FromBase64String(mail.Adjunto),
                        AttachmentName = mail.Nombre
                    });
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                //response.ReasonPhrase = string.Format("File not found: {0} .", docFile);
                throw new HttpResponseException(response);
            }
        }

        [HttpPost]
        [Route("api/PlanoDeCarga/AgregarExportador")]
        public HttpResponseMessage AgregarExportador(ExportadorDto exportador)
        {
            comandos.Ejecutar(new CrearExportador { Dto = exportador });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Route("api/PlanoDeCarga/ModificarExportador")]
        public HttpResponseMessage ModificarExportador(ExportadorDto exportador)
        {
            comandos.Ejecutar(new ModificarExportador { Dto = exportador });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Route("api/PlanoDeCarga/EliminarExportador")]
        public HttpResponseMessage EliminarExportador(int exportadorId)
        {
            var resultado = comandos.Ejecutar(new EliminarExportador { Id = exportadorId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }

        [HttpPost]
        [Route("api/PlanoDeCarga/ModificarCargadoPlanoDeCarga")]
        public HttpResponseMessage ModificarCargadoPlanoDeCarga(int planoDeCargaId)
        {

            comandos.Ejecutar(new ModificarCargadoPlanoDeCarga { Id = planoDeCargaId });
            Respuesta res = new Respuesta((int)HttpStatusCode.OK, "success");
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet]
        [Route("api/PlanoDeCarga/ListarBalanzadaBuque")]
        public HttpResponseMessage ListarBalanzadaBuque(int buque)
        {
            try
            {

                return Request.CreateResponse(HttpStatusCode.OK,
                                servicio.ListarBalanzadaBuque(buque)
                            );
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException);
            }

        }


        [HttpGet]
        [Route("api/PlanoDeCarga/listarCargaBalanza")]
        public HttpResponseMessage ListarCargaBalanzaPuerto()
        {
  
            return Request.CreateResponse(HttpStatusCode.OK,
                            servicio.ListarCargaBalanzaPuerto()
                        );
        }
    }
}