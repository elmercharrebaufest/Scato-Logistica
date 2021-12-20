using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class RefrescarOrdenDeCargaContenedor : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        public OutArgument<OrdenDeCargaContenedorDto> Orden { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);

            var logActividad = new LogActividadDto { Actividad = "Refrescar Orden de Carga Contenedor", ActividadXaml = "RefrescarOrdenDeCargaContenedor", WorkflowInstanceId = workflowId };
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
                Orden.Set(context, repositorio.ObtenerOrdenDeCargaContenedorPorInstanceId(workflowId));
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }
            return resultado;
        }
    }
}
