using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebPuertoApi.Atributos;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Molinos.Scato.WebPuertoApi.Controllers
{
    [Autorizacion(PermisosScato.PreLineUp)]
    public class EmbarqueController : BaseController
    {
        private readonly IServicioActividadFactory<IIngresarEmbarqueService> factory;
        private readonly IServicioComandos comandos;
        private readonly IListaDeWorkflows workflows;

        public EmbarqueController(IServicioActividadFactory<IIngresarEmbarqueService> factory, 
            IServicioRepositorio servicio, 
            IServicioComandos comandos,
            IListaDeWorkflows workflows) : base(servicio)
        {
            this.factory = factory;
            this.comandos = comandos;
            this.workflows = workflows;
        }

        [HttpPost]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/AltaEmbarque")]
        public HttpResponseMessage AltaEmbarque(EmbarqueDto embarque)
        {
            var datosEmbarque = 0;
            var workflow = ConfigurationManager.AppSettings["Workflow"];
            var centro = int.Parse(ConfigurationManager.AppSettings["Centro"]);
            try
            {
                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);

                IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, true, false, false, false);
                datosEmbarque = IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, false, true, false, false);
                IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, false, false, true, false);
                IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, false, false, false, true);

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, $"Error al crear el embarque, verifique que exista el centro ${centro} y el workflow ${workflow}");
            }
            return Request.CreateResponse(HttpStatusCode.OK, datosEmbarque);
        }

        private int IngresarEmbarque(EmbarqueDto embarque, 
            int workflowDefinicionId,
            IIngresarEmbarqueService servicioWf,
            bool vicentin,
            bool sanBenito,
            bool noryon,
            bool otrosMuelles)
        {
            var datosEmbarque = 0;
            var workflow = ConfigurationManager.AppSettings["Workflow"];
            var centro = int.Parse(ConfigurationManager.AppSettings["Centro"]);
            var controlRecorrido = new ControlRecorridoDto
            {
                Actividad = Textos.ActIngresarEmbarque,
                ActividadXaml = "IngresarEmbarque",
                NombreUsuario = nombreUsuario
            };
            embarque.CentroId = centro;
            embarque.Patente = embarque.NombreBuque;
            if ((vicentin && vicentin == embarque.Vicentin) || 
                (sanBenito && sanBenito == embarque.SanBenito) || 
                (noryon && noryon == embarque.Noryon) ||
                (otrosMuelles && otrosMuelles == embarque.OtrosMuelles))
            {
                var embarqueAux = (EmbarqueDto)embarque.Clone();
                embarqueAux.Vicentin = vicentin;
                embarqueAux.SanBenito = sanBenito;
                embarqueAux.Noryon = noryon;
                embarqueAux.OtrosMuelles = otrosMuelles;
                var datosEmbarqueAux = servicioWf.IngresarEmbarque(embarqueAux, workflow, workflowDefinicionId, controlRecorrido);
                datosEmbarque = ((ResultadoCrear)datosEmbarqueAux).Id;
            }

            return datosEmbarque;
        }


        [HttpGet]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ListarMateriales")]
        public HttpResponseMessage ListarMateriales()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListaMaterialesPuertoConDescripcionCorta()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ListarAgenciasMaritimas")]
        public HttpResponseMessage ListarAgenciasMaritimas()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarAgenciasMaritimas()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ListarCoordinadores")]
        public HttpResponseMessage ListarCoordinadores()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarCoordinadores()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ObtenerEmbarque")]
        public HttpResponseMessage ObtenerEmbarque(int id)
        {
            var embarque = servicio.ObtenerEmbarque(id);

            return Request.CreateResponse(HttpStatusCode.OK,
                embarque
            );
        }

        [HttpPost]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ModificarEmbarque")]
        public HttpResponseMessage ModificarEmbarque(EmbarqueDto embarque)
        {
            var embarqueDb = servicio.ObtenerEmbarque(embarque.Id);
            if (CantidadDeDestinos(embarque) == 1)
            {
                comandos.Ejecutar(new ModificarEmbarque { Dto = embarque });
            }
            else
            {
                //Modificar existente
                if (embarque.Vicentin && embarqueDb.Vicentin)
                {
                    ModificarEmbarque(embarque, true, false, false, false);
                }
                if(embarque.SanBenito && embarqueDb.SanBenito)
                {
                    ModificarEmbarque(embarque, false, true, false, false);
                }
                if(embarque.Noryon && embarqueDb.Noryon)
                {
                    ModificarEmbarque(embarque, false, false, true, false);
                }
                if (embarque.OtrosMuelles && embarqueDb.OtrosMuelles)
                {
                    ModificarEmbarque(embarque, false, false, false, true);
                }
                //Crear Nuevo
                var workflow = ConfigurationManager.AppSettings["Workflow"];
                var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(workflow);
                var servicioWf = factory.CrearServicio(workflowDefinicionId);
                if (embarque.Vicentin && !embarqueDb.Vicentin)
                {
                    IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, true, false, false, false);
                }
                if (embarque.SanBenito && !embarqueDb.SanBenito)
                {
                    IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, false, true, false, false);
                }
                if (embarque.Noryon && !embarqueDb.Noryon)
                {
                    IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, false, false, true, false);
                }
                if (embarque.OtrosMuelles && !embarqueDb.OtrosMuelles)
                {
                    IngresarEmbarque(embarque, workflowDefinicionId, servicioWf, false, false, false, true);
                }
                //Eliminar
                if (!embarque.Vicentin && embarqueDb.Vicentin)
                {
                    WorkflowController.Eliminar(servicio, comandos, workflows, nombreUsuario, embarqueDb.InstanciaWorkflow);
                }
                if (!embarque.SanBenito && embarqueDb.SanBenito)
                {
                    WorkflowController.Eliminar(servicio, comandos, workflows, nombreUsuario, embarqueDb.InstanciaWorkflow);
                }
                if (!embarque.Noryon && embarqueDb.Noryon)
                {
                    WorkflowController.Eliminar(servicio, comandos, workflows, nombreUsuario, embarqueDb.InstanciaWorkflow);
                }
                if (!embarque.OtrosMuelles && embarqueDb.OtrosMuelles)
                {
                    WorkflowController.Eliminar(servicio, comandos, workflows, nombreUsuario, embarqueDb.InstanciaWorkflow);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private void ModificarEmbarque(EmbarqueDto embarque,
            bool vicentin,
            bool sanBenito,
            bool noryon,
            bool otrosMuelles)
        {
            if ((vicentin && vicentin == embarque.Vicentin) ||
                (sanBenito && sanBenito == embarque.SanBenito) ||
                (noryon && noryon == embarque.Noryon) ||
                (otrosMuelles && otrosMuelles == embarque.OtrosMuelles))
            {
                var embarqueAux = (EmbarqueDto)embarque.Clone();
                embarqueAux.Vicentin = vicentin;
                embarqueAux.SanBenito = sanBenito;
                embarqueAux.Noryon = noryon;
                embarqueAux.OtrosMuelles = otrosMuelles;
                comandos.Ejecutar(new ModificarEmbarque { Dto = embarqueAux });
            }
        }

        private int CantidadDeDestinos(EmbarqueDto embarque)
        {
            return (embarque.Noryon ? 1 : 0) + (embarque.Vicentin ? 1 : 0) + (embarque.SanBenito ? 1 : 0) + (embarque.OtrosMuelles ? 1 : 0);
        }

        [HttpGet]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ListarATAPuerto")]
        public HttpResponseMessage ListarATAPuerto()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarATAPuerto()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ListarTipoDeBuquePuerto")]
        public HttpResponseMessage ListarTipoDeBuquePuerto()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarTipoDeBuquePuerto()
            );
        }

        [HttpGet]
        [Autorizacion(PermisosScato.PreLineUp)]
        [Route("api/Embarque/ListarUbicacionDeBuquePuerto")]
        public HttpResponseMessage ListarUbicacionDeBuquePuerto()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                servicio.ListarUbicacionDeBuquePuerto()
            );
        }

        [HttpGet]
        [Route("api/Embarque/ListarMotivosLimpieza")]
        public HttpResponseMessage ListarMotivosLimpieza()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
            servicio.ListarMotivosLimpieza()
            );
        }

        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/AgregarAgenciaMaritimaPuerto")]
        public HttpResponseMessage AgregarAgenciaMaritimaPuerto(AgenciaMaritimaPuertoDto agencia)
        {
            comandos.Ejecutar(new CrearAgenciaMaritimaPuerto { Dto = agencia });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/ModificarAgenciaMaritimaPuerto")]
        public HttpResponseMessage ModificarAgenciaMaritimaPuerto(AgenciaMaritimaPuertoDto agencia)
        {
            comandos.Ejecutar(new ModificarAgenciaMaritimaPuerto { Dto = agencia });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/EliminarAgenciaMaritimaPuerto")]
        public HttpResponseMessage EliminarAgenciaMaritimaPuerto(int agenciaId)
        {
            var resultado = comandos.Ejecutar(new EliminarAgenciaMaritimaPuerto { Id = agenciaId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }


        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/AgregarCoordinadorPuerto")]
        public HttpResponseMessage AgregarCoordinadorPuerto(CoordinadorPuertoDto coordinador)
        {
            comandos.Ejecutar(new CrearCoordinadorPuerto { Dto = coordinador });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/ModificarCoordinadorPuerto")]
        public HttpResponseMessage ModificarCoordinadorPuerto(CoordinadorPuertoDto coordinador)
        {
            comandos.Ejecutar(new ModificarCoordinadorPuerto { Dto = coordinador });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/EliminarCoordinadorPuerto")]
        public HttpResponseMessage EliminarCoordinadorPuerto(int coordinadorId)
        {
            var resultado = comandos.Ejecutar(new EliminarCoordinadorPuerto { Id = coordinadorId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }


        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/AgregarATAPuerto")]
        public HttpResponseMessage AgregarATAPuerto(ATAPuertoDto ata)
        {
            comandos.Ejecutar(new CrearATAPuerto { Dto = ata });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/ModificarATAPuerto")]
        public HttpResponseMessage ModificarATAPuerto(ATAPuertoDto ata)
        {
            comandos.Ejecutar(new ModificarATAPuerto { Dto = ata });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Autorizacion(PermisosScato.LineUp)]
        [Route("api/Embarque/EliminarATAPuerto")]
        public HttpResponseMessage EliminarATAPuerto(int ataId)
        {
            var resultado = comandos.Ejecutar(new EliminarATAPuerto { Id = ataId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }

        [HttpPost]
        [Route("api/Embarque/AgregarMotivosLimpieza")]
        public HttpResponseMessage AgregaMotivosLimpieza(MotivosLimpiezaDto motivosLimpieza)
        {
            comandos.Ejecutar(new CrearMotivosLimpieza { Dto = motivosLimpieza });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Route("api/Embarque/ModificarMotivosLimpieza")]
        public HttpResponseMessage ModificarMotivosLimpieza(MotivosLimpiezaDto motivosLimpieza)
        {
            comandos.Ejecutar(new ModificarMotivosLimpieza { Dto = motivosLimpieza });
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost]
        [Route("api/Embarque/EliminarMotivosLimpieza")]
        public HttpResponseMessage EliminarMotivosLimpieza(int motivosLimpiezaId)
        {
            var resultado = comandos.Ejecutar(new EliminarMotivosLimpieza { Id = motivosLimpiezaId });
            return Request.CreateResponse(!resultado.HayErrores ? true : false);
        }
    }
}