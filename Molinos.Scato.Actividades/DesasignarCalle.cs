using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Configuration;

namespace Molinos.Scato.Actividades
{
    public class DesasignarCalle : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();

            try
            {
                servicio.Ejecutar(new CrearLogActividad { Dto = new LogActividadDto
                {
                    Actividad = "DesasignarCalle",
                    ActividadXaml = "DesasignarCalle",
                    WorkflowInstanceId = context.WorkflowInstanceId,
                    Fecha = DateTime.Now
                }
                });
            }
            catch (Exception)
            {
            }

            var comentario = "ok";
            try
            {
                servicio.Ejecutar(new Dominio.Comandos.DesasignarCalle
                {
                    UltimaAsignacionId = 0,
                    InstanciaWorkflow = context.WorkflowInstanceId
                });
            }
            catch(Exception e)
            {
                comentario = e.Message;
            }

            try
            {
                if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                {
                    var srv = context.GetExtension<IServicioComandos>();
                    srv.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "DesasignarCalle",
                            Fecha = DateTime.Now,
                            Comentario = comentario,
                            NombreUsuario = "",
                            WorkflowInstanceId = context.WorkflowInstanceId,
                        }
                    });
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
