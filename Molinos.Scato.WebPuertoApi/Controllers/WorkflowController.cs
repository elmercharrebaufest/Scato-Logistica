using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebPuertoApi.Atributos;
using Molinos.Scato.WebPuertoApi.Helper;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Molinos.Scato.WebPuertoApi.Controllers
{
    public class WorkflowController : BaseController
    {
        private readonly IListaDeWorkflows workflows;
        private readonly IServicioComandos servicioComandos;

        public WorkflowController(IServicioRepositorio servicio, IListaDeWorkflows workflows, IServicioComandos servicioComandos) : base(servicio)
        {
            this.workflows = workflows;
            this.servicioComandos = servicioComandos;
        }

        [Autorizacion(PermisosScato.LineUpLectura)]
        [HttpGet]
        [Route("api/Workflow/Listar")]
        public HttpResponseMessage Listar()
        {
            var embarques = workflows.ListarEmbarques();
            return Request.CreateResponse(HttpStatusCode.OK, embarques);
        }

        [HttpGet]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Workflow/ListarEnLineUp")]
        public HttpResponseMessage ListarEnLineUp()
        {
            var embarques = workflows.ListarEmbarques("LineUp");
            var ubicaciones = servicio.ListarUbicacionDeBuquePuerto();

            return Request.CreateResponse(HttpStatusCode.OK,
                            embarques
                                .Where(y => y.Embarque.SanBenito)
                                .Select(x => new EmbarqueNavDto {
                                    Id = x.Embarque.Id,
                                    PlanoDeCargaId = x.LineUp.PlanoDeCarga.Id,
                                    ModuloDeCargaId = x.LineUp.ModuloDeCarga.Id,
                                    NombreBuque = x.Embarque.NombreBuque,
                                    Cargado = x.LineUp.PlanoDeCarga != null && x.LineUp.PlanoDeCarga.Cargado,
                                    NombreUbicacion = ubicaciones.Where(z => z.Id == x.Embarque.Ubicacion).FirstOrDefault()?.Nombre,
                                    EsLiquido = x.Embarque.EsLiquido
                                }).ToList()
                        );
        }

        [HttpGet]
        [Route("api/Workflow/Eliminar")]
        [Autorizacion(PermisosScato.LineUp)]
        public HttpResponseMessage Eliminar(Guid id)
        {
            var resultado = Eliminar(servicio, servicioComandos, workflows, nombreUsuario, id);
            return string.IsNullOrEmpty(resultado) ? Request.CreateResponse(HttpStatusCode.OK): Request.CreateResponse(HttpStatusCode.InternalServerError, resultado);
        }

        public static string Eliminar(IServicioRepositorio servicio, IServicioComandos servicioComandos, IListaDeWorkflows workflows, string nombreUsuario, Guid id)
        {
            var recorridoId = servicio.ObtenerRecorridoIdPorGuid(id);
            var resultado = servicioComandos.Ejecutar(new EliminarRecorrido { Id = recorridoId, NombreUsuario = nombreUsuario }) as ResultadoEliminarRecorrido;
            if (resultado != null && resultado.HayErrores)
            {
                return resultado.Errores.Values.First();
            }
            if (workflows.VerificarExistenciaDeWorkflowPorGuid(id))
            {
                var resultadoWf = workflows.EliminarInstanciaWorkflow(id);
                if (resultadoWf != null && resultadoWf.HayErrores)
                {
                    return resultadoWf.Errores.Values.First();
                }
            }
            return null;
        }
    }
}