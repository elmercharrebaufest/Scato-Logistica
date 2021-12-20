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
    public class EnviarMensajeV2 : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Mensaje { get; set; }

        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public InArgument<string> NumeroPrograma { get; set; }
        public InArgument<string> NumeroTrama { get; set; }
        public InArgument<string> NumeroVariable { get; set; }
        public InArgument<int> SegundosDeEspera { get; set; }

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
                var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
                var numeroPrograma = NumeroPrograma.Get<string>(context);
                var numeroTrama = NumeroTrama.Get<string>(context);
                var numeroVariable = NumeroVariable.Get<string>(context);
                var segundosDeEspera = SegundosDeEspera.Get<int>(context);


                var resultado = servicio.Ejecutar(new EnviarMensajeCarteLed
                    {
                        Mensaje = mensaje,
                        PuestoDeTrabajoId = puestoDeTrabajoId,
                        NumeroPrograma = numeroPrograma,
                        NumeroTrama = numeroTrama,
                        NumeroVariable = numeroVariable,
                        SegundosDeEspera = segundosDeEspera
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
