using System;
using System.Activities;
using System.Configuration;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class EnviarMensajeCircular : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Mensaje { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                servicio.Ejecutar(new CrearLogActividad
                {
                    Dto = new LogActividadDto
                    {
                        Actividad = "EnviarMensajeCircular",
                        ActividadXaml = "EnviarMensajeCircular",
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
                var resultado = servicio.Ejecutar(new EnviarMensajeCamioneroCircular
                {
                    CartaPorte = repositorio.ObtenerNumeroCartaPortePorGuid(context.WorkflowInstanceId),
                    Mensaje = Mensaje.Get<string>(context)
                });
            }
            catch (Exception e)
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
                            Actividad = "EnviarMensajeCircular",
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
