using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    /// <summary>
    ///     Retorna True solo si el transportista existe
    /// </summary>
    public class VerificacionTransportistaExiste : CodeActivity<bool>
    {
        [RequiredArgument]
        public InArgument<int?> TransportistaId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>(); 
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var workflowId = WorkflowId.Get<Guid>(context); 
            var transportistaId = TransportistaId.Get<int?>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto { Actividad = "Verificacion Transportista Existe", ActividadXaml = "VerificacionTransportistaExiste", WorkflowInstanceId = workflowId };
            logActividad.Fecha = DateTime.Now;
            var resultado = new Resultado();
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "VerificacionTransportistaExiste", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return transportistaId.HasValue && repositorio.ObtenerTransportista(transportistaId.Value) != null;
        }
    }
}