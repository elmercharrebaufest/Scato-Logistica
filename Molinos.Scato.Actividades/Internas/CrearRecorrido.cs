using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearRecorrido : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceIdViejo { get; set; }
        [RequiredArgument]
        public InArgument<Guid> InstanceIdNuevo { get; set; }
        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }
        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }
        public InArgument<string> NombreUsuario { get; set; }
        public OutArgument<RecorridoDto> Recorrido { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var servicioRepositorio = context.GetExtension<IServicioRepositorio>();
                var instanceIdViejo = InstanceIdViejo.Get<Guid>(context);
                var instanceIdNuevo = InstanceIdNuevo.Get<Guid>(context);
                var nombreWorkflow = NombreWorkflow.Get<string>(context);
                var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
                var nombreUsuario = NombreUsuario.Get<string>(context);
                resultado = servicioComandos.Ejecutar(new Dominio.Comandos.CrearRecorrido { InstanceId = instanceIdNuevo, InstanceIdViejo = instanceIdViejo, NombreWorkflow = nombreWorkflow, WorkflowDefinicionId = workflowDefinicionId, Usuario = nombreUsuario });
                Recorrido.Set(context, servicioRepositorio.ObtenerRecorridoPorGuid(instanceIdNuevo));
                if (resultado.HayErrores)
                {
                    resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.Recorrido_ErrorAlCrear);
            }
            return resultado;
        }
    }
}
