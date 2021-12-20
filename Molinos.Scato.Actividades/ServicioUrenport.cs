using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ServicioUrenport : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CartaPorteId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var cartaPorteId = CartaPorteId.Get<int>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad
                {
                    Dto = new LogActividadDto
                    {
                        Actividad = "ServicioUrenport",
                        ActividadXaml = "ServicioUrenport",
                        WorkflowInstanceId = workflowId,
                        Fecha = DateTime.Now
                    }
                });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            try
            {
                var resultadoEnvio = servicio.Ejecutar(new EnviarCartaPorteUnreport { CartaPorteId = cartaPorteId, WorkflowId = workflowId });

                if (!resultadoEnvio.HayErrores)
                {
                    servicio.Ejecutar(new EnviarTicketPesadaUnreport { WorkflowId = workflowId });
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ServicioUrenport", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}
