using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class ObtenerCalleInterna : CodeActivity
    {
        public OutArgument<string> Calle { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                servicio.Ejecutar(new CrearLogActividad { Dto = new LogActividadDto
                {
                    Actividad = "ObtenerCalleInterna",
                    ActividadXaml = "ObtenerCalleInterna",
                    WorkflowInstanceId = context.WorkflowInstanceId,
                    Fecha = DateTime.Now
                }
                });
            }
            catch (Exception)
            {
            }

            try
            {
                var calle = repositorio.ObtenerCalleInterna(context.WorkflowInstanceId);
                Calle.Set(context, calle);
            }
            catch
            {
            }
        }
    }
}
