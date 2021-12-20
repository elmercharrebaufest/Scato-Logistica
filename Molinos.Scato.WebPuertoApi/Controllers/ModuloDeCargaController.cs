using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebPuertoApi.Atributos;
using Molinos.Scato.WebPuertoApi.EXCEL;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Molinos.Scato.WebPuertoApi.Controllers
{
    public class ModuloDeCargaController : BaseController
    {
        private readonly IServicioComandos comandos;

        public ModuloDeCargaController(IServicioActividadFactory<IIngresarEmbarqueService> factory,
            IServicioRepositorio servicio, IServicioComandos comandos) : base(servicio)
        {
            this.comandos = comandos;
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ListarSentidoManoDeEmbarques")]
        public HttpResponseMessage ListarSentidoManoDeEmbarques()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarSentidoManoDeEmbarques());
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ListarCeldaManoDeEmbarques")]
        public HttpResponseMessage ListarCeldaManoDeEmbarques()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarCeldaManoDeEmbarques());
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/GuardarModuloDeCarga")]
        public HttpResponseMessage GuardarModuloDeCarga(ModuloDeCargaDto moduloDeCarga)
        {
            comandos.Ejecutar(new GuardarModuloDeCarga { Dto = moduloDeCarga });
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ObtenerModuloDeCarga")]
        public HttpResponseMessage ObtenerModuloDeCarga(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ObtenerModuloDeCarga(id));
        }

        [HttpGet]
        [Route("api/ModuloDeCarga/ObtenerUltimaHabilitacionDeTanques")]
        public HttpResponseMessage ObtenerUltimaHabilitacionDeTanques()
        {
            try
            {
                var habilitacion = servicio.ObtenerUltimaHabilitacionDeTanques();
                return Request.CreateResponse(HttpStatusCode.OK, habilitacion);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ListarMotivosDeCorte")]
        public HttpResponseMessage ListarMotivosDeCorte()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarMotivosDeCorte());
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ListarMotivosFallasBalanza")]
        public HttpResponseMessage ListarMotivosFallasBalanza()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarMotivosFallasBalanza());
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ListarTurnoPuerto")]
        public HttpResponseMessage ListarTurnoPuerto()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarTurnoPuerto());
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ObtenerCantidadCubitacionDeTanques")]
        public HttpResponseMessage ObtenerCantidadCubitacionDeTanques(string tk, double altura)
        {
            return Request.CreateResponse(HttpStatusCode.OK, servicio.ObtenerCantidadCubitacionDeTanques(tk, altura)
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ObtenerLlenadoMilimetroPorTanque")]
        public HttpResponseMessage ObtenerLlenadoMilimetroPorTanque(string cm, string mm, string tanqueNum)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, servicio.ObtenerLlenadoMinimetroPorTanque(cm, mm, tanqueNum));
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ObtenerDensidadPorTemperaturaDeMaterial")]
        public HttpResponseMessage ObtenerDensidadPorTemperaturaDeMaterial(int materialPuertoId, int grado)
        {

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, servicio.ObtenerDensidadPorTemperaturaDeMaterial(materialPuertoId, grado));
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/GuardarPlanillaDeTurnos")]
        public HttpResponseMessage GuardarPlanillaDeTurnos(int IdModuloDeCarga, ModuloDeCargaPlanillaDeTurnosDto planillaDeTurnosDto)
        {
            try
            {
                var resultado = new ResultadoPrevisualizar();
                 comandos.Ejecutar(new GuardarPlanillaDeTurnos { Dto = planillaDeTurnosDto, IdModuloDeCarga = IdModuloDeCarga });

                string destinatarios = servicio.obtenerDireccionesDeMail("PlanillaDeTurnos");
                System.Collections.Generic.List<string> dest = new System.Collections.Generic.List<string>();
                string fechaTurno = planillaDeTurnosDto.Fecha.Value.ToString("yyyyMMdd");
                foreach (string mail in destinatarios.Split(';'))
                    dest.Add(mail);

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                    var docFile = "Planilla de turnos" + fechaTurno + ".xls";
                    try
                    {
                    var generadorExcel = new ExcelLiquido();
                   generadorExcel.GenerarArchivo(resultado, planillaDeTurnosDto, IdModuloDeCarga);
                   
          
                    // CARACTERES NO IMPRIMIBLES:
                    // Enter: (\n -> <br/>)
                    // Tabulador: (\t -> &nbsp;&nbsp;&nbsp;&nbsp;)
                    // Negrita: (\f -> <b>) (\f\f -> </b>)
                    // Subrayado: (\0 -> <u>) (\0\0 -> </u>)
                    comandos.Ejecutar(new EnvioMail
                        {
                            Cuerpo = "Planilla del dia "+ planillaDeTurnosDto.Fecha,
                            Destinatarios = dest,
                            Titulo = $"Planilla de turnos Modulo de carga "+IdModuloDeCarga + fechaTurno,
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
                



                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/ModuloDeCarga/ActualizarEstadoBuque")]
        public HttpResponseMessage ActualizarEstadoBuque(int Embarque_Id, int Estado)
        {
            servicio.ActualizarEstadoBuque(Embarque_Id, Estado);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}