using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearEmbarque : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InOutArgument<EmbarqueDto> Embarque { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreWorkflow { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }

        [RequiredArgument]
        public InArgument<int> WorkflowDefinicionId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> InstanciaWorkflowId { get; set; }
       
        protected override Resultado Execute(CodeActivityContext context)
        {
            var embarque = Embarque.Get<EmbarqueDto>(context);
            var nombreWorkflow = NombreWorkflow.Get<string>(context);
            var instanciaWorkflow = InstanciaWorkflowId.Get<Guid>(context);
            var workflowDefinicionId = WorkflowDefinicionId.Get<int>(context);
            var nombreUsuario = NombreUsuario.Get<string>(context);
            var resultado = new ResultadoCrearWorkflow();
            resultado.InstanciaWorkflowId = InstanciaWorkflowId.Get(context);
            try
            {
                var servicioComandos = context.GetExtension<IServicioComandos>();
                var srvRepositorio = context.GetExtension<IServicioRepositorio>();
                var resultadoCrear = servicioComandos.Ejecutar(new Dominio.Comandos.CrearEmbarque
                {
                    Embarque = embarque,
                    NombreWorkflow = nombreWorkflow,
                    InstanciaWorkflowId = instanciaWorkflow,
                    WorkflowDefinicionId = workflowDefinicionId,
                    Usuario = nombreUsuario
                }) as ResultadoCrear;
                resultado.Id = resultadoCrear.Id;

                var embarqueDto = srvRepositorio.ObtenerEmbarque(resultado.Id);
                if (embarqueDto != null)
                {
                    Embarque.Set(context, embarqueDto);
                }
                if (resultadoCrear.HayErrores)
                {
                    resultado.Errores.Add("", Textos.Error_ActualizarGenerico);
                }
            }
            catch (Exception)
            {
                resultado.Errores.Add("1", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }
    }
}
