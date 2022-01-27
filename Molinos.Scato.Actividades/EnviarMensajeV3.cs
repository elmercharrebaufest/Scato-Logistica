using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class EnviarMensajeV3 : CodeActivity
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

            try
            {
                var mensaje = Mensaje.Get<string>(context);
                var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
                var numeroPrograma = NumeroPrograma.Get<string>(context);
                var numeroTrama = NumeroTrama.Get<string>(context);
                var numeroVariable = NumeroVariable.Get<string>(context);
                var segundosDeEspera = SegundosDeEspera.Get<int>(context);

                servicio.Ejecutar(new EnviarMensajeAsincronoCartelLed
                {
                    Mensaje = mensaje,
                    PuestoDeTrabajoId = puestoDeTrabajoId,
                    NumeroPrograma = numeroPrograma,
                    NumeroTrama = numeroTrama,
                    NumeroVariable = numeroVariable,
                    SegundosDeEspera = segundosDeEspera,
                    WorkflowInstanceId = context.WorkflowInstanceId
                });
            }
            catch (Exception)
            {
            }
        }
    }
}
