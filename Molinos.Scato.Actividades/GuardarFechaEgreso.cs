using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class GuardarFechaEgreso : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public OutArgument<DateTime> FechaEgreso { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var workflowId = WorkflowId.Get<Guid>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var fecha = DateTime.Now;

            var logActividad = new LogActividadDto { Actividad = "Guardar Fecha Egreso", ActividadXaml = "GuardarFechaEgreso", WorkflowInstanceId = context.WorkflowInstanceId };
            logActividad.Fecha = DateTime.Now;
            var resultado = new Resultado();
            try
            {
                resultado = servicioComandos.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            servicioComandos.Ejecutar(new ModificarFechaEgreso { Fecha = fecha, WorkflowInstanciaId = context.WorkflowInstanceId});
            FechaEgreso.Set(context,fecha);

            try
            {
                resultado = servicioComandos.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "GuardarFechaEgreso", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.FinDeActividad_ErrorEnLaCarga);
            }
        }
    }
}
