using System;
using System.Activities;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class InvocarNuevoWorkflow : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<string> CodigoNuevoWorkflow { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        public InArgument<string> NombreUsuario { get; set; }
        protected override Resultado Execute(CodeActivityContext context)
        {
            var comando = context.GetExtension<IServicioComandos>();
            try
            {
                comando.Ejecutar(new CrearLogActividad { Dto = new LogActividadDto { Actividad = "Invocando Nuevo Workflow", WorkflowInstanceId = context.WorkflowInstanceId } });
            }
            catch (Exception)
            {
            }
            var codigoNuevoWorkflow = CodigoNuevoWorkflow.Get<string>(context);
            var centroId = CentroId.Get<int>(context);
            var usuario = NombreUsuario.Get<string>(context);
            var servicio = context.GetExtension<IServicioRepositorio>();
            var workflowDefinicionId = servicio.ObtenerUltimaWorkflowDefinicionPorCordigo(codigoNuevoWorkflow);
            var uri = ConfigurationManager.AppSettings["UrlBaseWorkflow"] + workflowDefinicionId + ".xamlx";
            var canal = new ChannelFactory<IEmpezarNuevoWorkflowService>(new BasicHttpBinding("CommonBinding"), new EndpointAddress(uri)).CreateChannel();
            var resultado = canal.EmpezarNuevoWorkflow(context.WorkflowInstanceId, centroId, codigoNuevoWorkflow, workflowDefinicionId, usuario);
            if (resultado.HayErrores)
            {
                comando.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InvocarNuevoWorkflow",
                        Fecha = DateTime.Now,
                        Comentario = resultado.Errores.FirstOrDefault().Value,
                        NombreUsuario = usuario,
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
                var id = servicio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);
                resultado = comando.Ejecutar(new EliminarRecorrido { Id = id });
            }
            return resultado;
        }
    }
}
