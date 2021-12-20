using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class RefrescarCartaDePorte : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        public OutArgument<CartaPorteDto> Orden { get; set; }
        public OutArgument<VehiculoDto> Vehiculo { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);

            var logActividad = new LogActividadDto { Actividad = "Refrescar Carta de Porte", ActividadXaml = "RefrescarCartaDePorte", WorkflowInstanceId = workflowId };
            logActividad.Fecha = DateTime.Now;
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
                Orden.Set(context,repositorio.ObtenerCartaPortePorInstanceId(workflowId));
                Vehiculo.Set(context, repositorio.ObtenerVehiculoPorGuid(workflowId));
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }
            return resultado;
        }
    }
}
