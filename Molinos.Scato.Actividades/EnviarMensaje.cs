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
    public class EnviarMensaje : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Mensaje { get; set; }

        [RequiredArgument]
        public InArgument<string> Codigo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            

            var logActividad = new LogActividadDto
            {
                Actividad = "EnviarMensaje",
                ActividadXaml = "EnviarMensaje",
                WorkflowInstanceId = context.WorkflowInstanceId,
                Fecha = DateTime.Now
            };
            try
            {
                servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
            }

            var comentario = "ok";
            try
            {
                var mensaje = Mensaje.Get<string>(context);
                var codigo = Codigo.Get<string>(context);
               


                var resultado = servicio.Ejecutar(new EnviarMensajeCarteLed
                    {
                        Mensaje = mensaje,
                        Codigo = codigo
                    });
                if (resultado.HayErrores)
                {
                    comentario = resultado.Errores.Values.First();
                }
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
                            Actividad = "EnviarMensaje",
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
