using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades.Internas
{
    public class InformarPagoTasaMunicipal : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InOutArgument<Guid> WorkflowInstanceId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var log = context.GetExtension<ILogger>();
            var workflowInstanceId = WorkflowInstanceId.Get<Guid>(context);
            
            var resultado = new Resultado();
            try
            {
                log.Debug($"Informando el pago de la tasa municipal para el recorrido {workflowInstanceId}.");
                var pagos = srvRepositorio.ObtenerPagosDigitalesPorInstanceId(workflowInstanceId);
                if (pagos.Any())
                {
                    log.Debug($"El recorrido {workflowInstanceId} tiene pagos digitales asociados.");
                    foreach (var pago in pagos)
                    {
                        servicioComandos.Ejecutar(new MOAPayInformarPagoComoConsumido
                        {
                            Id = pago.IdMOAPay,
                            Disponible = "N",
                            IdIntance = workflowInstanceId
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error inesperado al informar el pago de la tasa municipal: {ex.Message}");
                resultado.Errores.Add("", Textos.Recrorrido_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
