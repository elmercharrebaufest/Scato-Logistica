using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ConversionKilosALitros : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNetoBodega { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public OutArgument<int> PesoNetoBodegaEnLitros { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();

            var workflowId = WorkflowId.Get<Guid>(context);
            var pesoNetoBodega = PesoNetoBodega.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto { Actividad = "Conversión Kilos a Litros", WorkflowInstanceId = context.WorkflowInstanceId };
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
            try
            {
                var resultadoComando = servicioComandos.Ejecutar(new ModificarRecorridoPesoNetoBodegaEnLitros { WorkflowId = workflowId, PesoNetoBodega = pesoNetoBodega, MaterialId = materialId }) as ResultadoModificarRecorridoPesoNetoBodegaEnLitros;
                PesoNetoBodegaEnLitros.Set(context, resultadoComando.PesoNetoBodegaEnLitros);
            }
            catch (Exception)
            {
                resultado.Errores.Add("errorComando", Textos.LogActividad_ErrorEnLaCarga);
            }
            try
            {
                resultado = servicioComandos.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ConversionKilosALitros", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.FinDeActividad_ErrorEnLaCarga);
            }
        }
    }
}
